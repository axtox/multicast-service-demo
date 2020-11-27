using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Receiver.Model;

namespace Receiver.Processors
{
    public class StockProcessing
    {
        #region MSDN Singleton implementation

        private static volatile StockProcessing _instance;
        private static readonly object SyncRoot = new object();

        public static StockProcessing Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new StockProcessing();
                    }
                }

                return _instance;
            }
        }

        #endregion

        private string _connectionString;
        private Timer _timer;

        private readonly object _fakeLossSyncObject = new object();
        private bool _fakeDataLossEnabled;

        private long _totalPackagesLost;
        private long _totalPackagesReceived;
        private long _receivedStocksTotal;

        private BlockingCollection<int> _valuesQueue = new BlockingCollection<int>(100000);

        protected StockProcessing()
        {
            _timer = new Timer(SendValues);
            _timer.Change(0, int.MaxValue);

            _connectionString = ConfigurationManager.ConnectionStrings["StockDB"].ConnectionString;
        }

        public async Task Initialize()
        {
            await MySqlHelper.ExecuteNonQueryAsync(_connectionString, "TRUNCATE TABLE profitcenter.stocks_test;");
        }

        public async Task<ReceiverReport> AddStock(int stock)
        {
            bool lossEnabled;
            lock (_fakeLossSyncObject)
            {
                lossEnabled = _fakeDataLossEnabled;
            }

            if (lossEnabled)
            {
                Interlocked.Increment(ref _totalPackagesLost);
            }
            else
            {
                _valuesQueue.Add(stock);

                Interlocked.Increment(ref _totalPackagesReceived);
                Interlocked.Add(ref _receivedStocksTotal, stock);
            }

            return new ReceiverReport
            {
                PackagesLost = Interlocked.Read(ref _totalPackagesLost),
                PackagesReceived = Interlocked.Read(ref _totalPackagesReceived)
            };
        }

        public void SetDelayTime(int delayInSeconds)
        {
            _timer.Change(0, delayInSeconds * 1000);
        }

        public async Task<Statistics> GetStatistics()
        {
            var received = Interlocked.Read(ref _totalPackagesReceived);
            var total = Interlocked.Read(ref _receivedStocksTotal);

            var average = total / received;
            var standardDeviation = MySqlHelper.ExecuteDataRowAsync(_connectionString, "SELECT STDDEV(value) from profitcenter.stocks_test;");
            var mode = MySqlHelper.ExecuteDataRowAsync(_connectionString, "SELECT value FROM profitcenter.stocks_test GROUP BY value ORDER BY COUNT(value) DESC LIMIT 1");
            var median = MySqlHelper.ExecuteDataRowAsync(_connectionString, $"SELECT value FROM profitcenter.stocks_test LIMIT {received/2},1;");
            return new Statistics
            {
                Average = average,
                Mode = (int)(await mode).ItemArray.First(), //dangerous use, only for demo purposes 
                Median = (int)(await median).ItemArray.First(), //dangerous use, only for demo purposes 
                StandardDeviation = (double)(await standardDeviation).ItemArray.First() //dangerous use, only for demo purposes 
            };
        }

        //TODO: careful with exceptions!
        private void SendValues(object state)
        {
            if (_valuesQueue.Count == 0)
                return;

            lock (_fakeLossSyncObject)
            {
                _fakeDataLossEnabled = true;
            }

            var bulkInsertQuery = new StringBuilder($"INSERT INTO profitcenter.stocks_test(value) VALUES ({_valuesQueue.Take()})");
            
            for (int i = 0; i < 100000; i++)
            {
                if(_valuesQueue.Count == 0)
                    break;

                bulkInsertQuery.Append(",");
                bulkInsertQuery.Append($"({_valuesQueue.Take()})");
            }
            bulkInsertQuery.Append(";");

            MySqlHelper.ExecuteNonQuery(_connectionString, bulkInsertQuery.ToString());

            lock (_fakeLossSyncObject)
            {
                _fakeDataLossEnabled = false;
            }
        }
    }
}

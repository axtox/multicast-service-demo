using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

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

        private object totalSyncObject = new object();
        private long totalPackagesReceived;

        private BlockingCollection<int> _valuesQueue = new BlockingCollection<int>(100000);

        protected StockProcessing()
        {
            _timer = new Timer(SendValues);
            _timer.Change(0, int.MaxValue);

            _connectionString = ConfigurationManager.ConnectionStrings["StockDB"].ConnectionString;
        }

        public async Task AddStock(int stock)
        {
            _valuesQueue.Add(stock);
            /*var databaseQuery = MySqlHelper.ExecuteNonQueryAsync(_connectionString,
                    $"INSERT INTO profitcenter.stocks_test(value) values({stock});");*/

            Interlocked.Increment(ref totalPackagesReceived);
            Console.Write($"\rPackages received: {totalPackagesReceived}");

            //await databaseQuery;

        }

        public void SetDelayTime(int delayInSeconds)
        {
            _timer.Change(0, delayInSeconds * 1000);
        }

        private async void SendValues(object state)
        {
            if (_valuesQueue.Count == 0)
                return;

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
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void AddStock(int stock)
        {

        }
    }
}

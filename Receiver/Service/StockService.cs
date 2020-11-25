using System;
using System.Threading.Tasks;
using Receiver.Processors;

namespace Receiver.Service
{
    public class StockService : IStockService
    {
        public async Task SendStock(int stock)
        {
            //thread-safe call
            await StockProcessing.Instance.AddStock(stock);
        }
    }
}

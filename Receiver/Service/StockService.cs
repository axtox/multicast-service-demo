using System;
using System.Threading.Tasks;
using Receiver.Managers;
using Receiver.Processors;

namespace Receiver.Service
{
    public class StockService : IStockService
    {
        public async Task SendStock(int stock)
        {
            //thread-safe call of Instance field
            var report = StockProcessing.Instance.AddStock(stock);

            //thread-safe call of Instance field
            ConsolePrintManager.Instance.PrintReport(await report);
        }
    }
}

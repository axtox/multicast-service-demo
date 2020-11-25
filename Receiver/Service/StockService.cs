using System;
using System.Threading.Tasks;

namespace Receiver.Service
{
    public class StockService : IStockService
    {
        public async Task SendStock(int stock)
        {
            Console.WriteLine($"Stock: Price: ${stock:C}");
        }
    }
}

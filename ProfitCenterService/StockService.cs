using System;

namespace ProfitCenterService
{
    public class StockService : IStockService
    {
        public void SendStockDetail(Stock stock)
        {
            Console.WriteLine($"Stock: Price: ${stock.Price:C}");
        }
    }
}

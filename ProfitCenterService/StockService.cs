using System;

namespace ProfitCenterService
{
    public class StockService : IStockService
    {
        public void SendStock(Stock stock)
        {
            Console.WriteLine($"Stock: Price: ${stock.Price:C}");
        }
    }
}

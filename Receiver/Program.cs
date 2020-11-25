using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Receiver.Service;

namespace Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(StockService));
            try
            {
                host.Open();

                Console.WriteLine($"Service `{nameof(StockService)}` hosted on:\n");
                foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
                    Console.WriteLine(endpoint.Address);

                Console.WriteLine("\n\nPress Enter key to stop service...");
                Console.ReadKey();

                host.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
                host.Abort();
            }
        }
    }
}

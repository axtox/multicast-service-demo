using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading;
using Transmitter.ServiceReference;

namespace Transmitter
{
    public class Program
    {
        private static Random _random = new Random();
        private static int randomMax;
        private static int randomMin;
        private static string TransmitterAddress;

        private static async Task Main(string[] args)
        {
            GetSettingsForRandom();
            GetSettingsForTransmitter();

            var client = new StockServiceClient();
            client.Endpoint.Address = new EndpointAddress($"soap.udp://{TransmitterAddress}:34197/StockService");

            Console.WriteLine("Immediately start stock transmitting..\n\n");

            while (true)
            {
                var stock = _random.Next(randomMin, randomMax);
                await client.SendStockAsync(stock); // no need for await?
            }

            ((IClientChannel)client).Close();
        }

        private static async Task Transmit(CancellationToken cancellation)
        {
            while (true)
            {

                cancellation.ThrowIfCancellationRequested();
            }
        } 

        private static void GetSettingsForRandom()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                int.TryParse(appSettings["StockMax"], out randomMax);
                int.TryParse(appSettings["StockMin"], out randomMin);
            }
            catch (ConfigurationErrorsException e)
            {
                Console.WriteLine($"Error reading app settings. {e.Message}");
            }
        }
        private static void GetSettingsForTransmitter()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                //no validation
                TransmitterAddress = appSettings["MulticastGroup"];
            }
            catch (ConfigurationErrorsException e)
            {
                Console.WriteLine($"Error reading app settings. {e.Message}");
            }
        }
    }
}

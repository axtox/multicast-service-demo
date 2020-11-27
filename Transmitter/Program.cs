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

        private static int _totalPackages;

        private static async Task Main(string[] args)
        {
            GetSettingsForRandom();
            GetSettingsForTransmitter();


            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.WriteLine("Press ESC key to stop transmitting.");

            var cancellation = new CancellationTokenSource();

            Console.SetCursorPosition(0, Console.WindowHeight / 2);
            Console.WriteLine("Immediately start stock transmitting...");

            Transmit(cancellation.Token); // no awaiting. fire and go


            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
            }

            cancellation.Cancel();

            //((IClientChannel)client).Close();
        }

        private static async Task Transmit(CancellationToken cancellation)
        {
            var client = new StockServiceClient();
            client.Endpoint.Address = new EndpointAddress($"soap.udp://{TransmitterAddress}:34197/StockService");

            while (true)
            {
                var stock = _random.Next(randomMin, randomMax);
                var sendTask = client.SendStockAsync(stock); // no need for await?

                cancellation.ThrowIfCancellationRequested();

                Interlocked.Increment(ref _totalPackages);
                
                await sendTask;

                var message = $"Packages sent {_totalPackages}";
                Console.SetCursorPosition(Console.WindowWidth - message.Length, 0);
                Console.Write(message);

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

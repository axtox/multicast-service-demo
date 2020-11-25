using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using Receiver.Processors;
using Receiver.Service;

namespace Receiver
{
    class Program
    {
        private static string ReceiverAddress;
        private static int DelayInSeconds;

        static async Task Main(string[] args)
        {
            GetSettingsForReceiver();

            var host = new ServiceHost(typeof(StockService));
            var serviceEndpoint = host.Description.Endpoints.First();
            serviceEndpoint.Address = new EndpointAddress($"soap.udp://{ReceiverAddress}:34197/StockService");

            StockProcessing.Instance.SetDelayTime(DelayInSeconds);

            try
            {
                host.Open();

                Console.WriteLine("\n\nPress Enter key to stop service...\n");
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

        private static void GetSettingsForReceiver()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                //no validation
                ReceiverAddress = appSettings["MulticastGroup"];
                DelayInSeconds = int.Parse(appSettings["DelayInSeconds"]);
            }
            catch (ConfigurationErrorsException e)
            {
                Console.WriteLine($"Error reading app settings. {e.Message}");
            }
        }
    }
}

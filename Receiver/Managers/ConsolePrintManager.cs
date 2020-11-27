using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Receiver.Model;

namespace Receiver.Managers
{
    public class ConsolePrintManager
    {
        #region MSDN Singleton implementation

        private static volatile ConsolePrintManager _instance;
        private static readonly object SyncRoot = new object();

        public static ConsolePrintManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new ConsolePrintManager();
                    }
                }

                return _instance;
            }
        }

        #endregion

        private readonly object _consolePrintLock = new object();

        protected ConsolePrintManager() { }

        public void PrintReport(ReceiverReport report)
        {
            lock (_consolePrintLock)
            {
                var receivedMessage = $"Packages received: { report.PackagesReceived}";
                Console.SetCursorPosition(Console.WindowWidth - receivedMessage.Length, 0);
                Console.WriteLine(receivedMessage);

                var lostMessage = $"Packages lost: { report.PackagesLost}";
                Console.SetCursorPosition(Console.WindowWidth - lostMessage.Length, 1);
                Console.WriteLine(lostMessage);
            }
        }

        public void PrintStatistics(Statistics statistics)
        {
            lock (_consolePrintLock)
            {
                var statisticTableNames = "AVERAGE        STANDARD DEVIATION        MODE        MEDIAN";
                Console.SetCursorPosition((Console.WindowWidth / 2) - statisticTableNames.Length / 2, (Console.WindowHeight / 2) + 1);
                Console.WriteLine(statisticTableNames);

                var statisticValues = $"{statistics.Average}\t\t{statistics.StandardDeviation:F2}\t\t\t{statistics.Mode}\t\t{statistics.Median}";
                Console.SetCursorPosition((Console.WindowWidth / 2) - statisticTableNames.Length / 2, Console.CursorTop);
                Console.WriteLine(statisticValues);
            }
        }

        public void PrintHelp()
        {
            lock (_consolePrintLock)
            {
                Console.Clear();
                
                Console.SetCursorPosition(0, Console.WindowHeight - 2);

                Console.WriteLine("Press Enter key to get statistics.");
                Console.WriteLine("Press ESC key to exit the application.");

                Console.SetCursorPosition(0, 0);
            }
        }

        public void PrintError(string message)
        {
            lock (_consolePrintLock)
            {
                Console.Clear();

                PrintMessage(message);
            }
        }

        public void PrintMessage(string message)
        {
            lock (_consolePrintLock)
            {
                Console.SetCursorPosition(0, Console.WindowHeight / 2);

                Console.Write(message);
            }
        }

        public void ClearMessage()
        {
            lock (_consolePrintLock)
            {
                Console.SetCursorPosition(0, Console.WindowHeight / 2);

                ClearCurrentLine();
            }
        }

        private void ClearCurrentLine()
        {
            Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
        }
    }
}

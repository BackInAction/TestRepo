using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BogusConsoleApplication
{
    class Program
    {
        private static Logger logger;

        static void Main(string[] args)
        {
            using (logger = new Logger())
            {
                logger.Open();
                Console.WriteLine("save me");
                CancellationTokenSource cts = new CancellationTokenSource();

                var ts = new TcpServer(5555, cts.Token, logger, Connection);
                ts.Initialize();
                ts.StartListener();
                Console.ReadLine();
                logger.Log("shutting down...");
                cts.Cancel();
                logger.Log("cancelled...");
                ts.StopListener();
                Console.ReadLine();
            }
        }

        private static bool Connection(TcpClient client, CancellationToken ct)
        {
            logger.Log("Connection made, woot!  Sleeping for 5s");
            Thread.Sleep(5000);
            logger.Log("Done sleeping...");
            var stream = client.GetStream();

            return false;
        }
    }
}

using Microsoft.Owin.Hosting;
using System;
using System.Net.Http;
using System.ServiceProcess;

namespace Replacer
{

    public class Program
    {
        #region Nested classes to support running as service
        public const string ServiceName = "Replacer.Service";

        public class Service : ServiceBase
        {
            public Service()
            {
                ServiceName = Program.ServiceName;
            }

            protected override void OnStart(string[] args)
            {
                Program.Start();
            }

            protected override void OnStop()
            {
                Program.Stop();
            }
        }
        #endregion
        static void Main()
        {
            if (!Environment.UserInteractive)
                // running as service
                using (var service = new Service())
                {
                    ServiceBase.Run(service);
                }
            else
            {
                // running as console app
                Start();

                Console.WriteLine("Press any key to stop...");
                Console.ReadKey(true);

                Stop();
            }
        }

        private static void Start()
        {
            string baseAddress = "http://localhost:9009/";
            WebApp.Start<Startup>(url: baseAddress);
        }

        private static void Stop()
        {
        }
    }
}

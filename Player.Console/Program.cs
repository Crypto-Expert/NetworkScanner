using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using NServiceBus;
using NetworkScanner.CLI.Chains;

namespace NetworkScanner.CLI
{
    static class Program
    {
        private static readonly List<IChain> Chains = new List<IChain>
        { 
            new LitecoinMainNet(),
            new DashMainNet()
        };
        private static ManualResetEvent ExitEvent = new ManualResetEvent(false);
        public static IEndpointInstance EndpointInstance = null;

        private static void SetupLogging()
        {
            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "app.log" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            // Apply config           
            LogManager.Configuration = config;
        }

        private static async Task SetupServiceBus()
        {
            var endpointConfiguration = new EndpointConfiguration("NetworkScanner");
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            EndpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

        }

        static async Task Main()
        {
            SetupLogging();
            await SetupServiceBus();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                ExitEvent.Set();
            };

            foreach (IChain chain in Chains)
            {
                await chain.Connect();
            }

            await WaitForShutdownAsync();
        }

        private static async Task WaitForShutdownAsync()
        {
            ExitEvent.WaitOne();
            foreach (IChain chain in Chains)
            {
                chain.Stop();
            }
            await EndpointInstance.Stop()
                .ConfigureAwait(false);
            Environment.Exit(0);
        }
    }
}
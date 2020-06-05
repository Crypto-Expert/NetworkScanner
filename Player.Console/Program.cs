using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using NetworkScanner.CLI.Chains;

namespace NetworkScanner.CLI
{
    static class Program
    {
        private static readonly List<BaseChain> Chains = new List<BaseChain>
        { 
            new DashMainNet()
        };

        private static ManualResetEvent ExitEvent = new ManualResetEvent(false);

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

        static async Task Main()
        {
            SetupLogging();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                ExitEvent.Set();
            };

            foreach (BaseChain chain in Chains)
            {
                await chain.Connect();
            }

            await WaitForShutdownAsync();
        }

        private static async Task WaitForShutdownAsync()
        {
            ExitEvent.WaitOne();
            foreach (BaseChain chain in Chains)
            {
                await chain.StopAsync();
            }
            Environment.Exit(0);
        }
    }
}
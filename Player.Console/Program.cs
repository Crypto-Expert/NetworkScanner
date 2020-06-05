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

        static async Task Main()
        {
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
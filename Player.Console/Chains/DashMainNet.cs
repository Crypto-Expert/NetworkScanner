using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using NBitcoin;
using NBitcoin.Altcoins;
using NBitcoin.Protocol;

namespace NetworkScanner.CLI.Chains
{
    public class DashMainNet : BaseChain
    {
        protected override List<NetworkAddress> BootstrapNodes => new List<NetworkAddress>()
        {
            new NetworkAddress(IPAddress.Parse("138.197.138.219"), 9999),
            new NetworkAddress(IPAddress.Parse("151.80.5.219"), 9999),
            new NetworkAddress(IPAddress.Parse("161.35.82.171"), 9999),
        };

        public override string ChainName => "dash.mainnet";
        public override Network Network => Dash.Instance.Mainnet;
        public override string ChainFile => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), ChainName);
    }
}

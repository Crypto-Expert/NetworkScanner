using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using NBitcoin;
using NBitcoin.Altcoins;
using NBitcoin.Protocol;

namespace NetworkScanner.CLI.Chains
{
    public class LitecoinMainNet : BaseChain
    {
        public static new List<NetworkAddress> BootstrapNodes = new List<NetworkAddress>()
        {
            new NetworkAddress(IPAddress.Parse("104.248.132.120"), 9333),

        };

        public override string ChainName => "litecoin.mainnet";
        public override Network Network => Litecoin.Instance.Mainnet;
        public override string ChainFile => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), ChainName);
    }
}
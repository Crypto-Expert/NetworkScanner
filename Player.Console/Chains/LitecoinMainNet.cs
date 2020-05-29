using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.Altcoins;
using NBitcoin.Protocol;
using NBitcoin.Protocol.Behaviors;
using NetworkScanner.CLI.Chains.MessageHandlers;

namespace NetworkScanner.CLI.Chains
{
    public class LitecoinMainNet : IChain
    {
        private static AddressManager AddressManager = new AddressManager();
        public static List<NetworkAddress> BootstrapNodes = new List<NetworkAddress>()
        {
            new NetworkAddress(IPAddress.Parse("104.248.132.120"), 9333),

        };

        public static SlimChain Chain = new SlimChain(GetNetwork().GenesisHash);
        private static Mutex _chainLock = new Mutex();

        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private int _height = 0;
        private NodesGroup _nodeGroup = new NodesGroup(GetNetwork(), GetNodeConnectionParameters());

        public static string GetChainName() => "litecoin.mainnet";
        public static Network GetNetwork() => Litecoin.Instance.Mainnet;
        public static string GetChainFile() => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), GetChainName());


        public static NodeConnectionParameters GetNodeConnectionParameters()
        {
            var parameters = new NodeConnectionParameters();
            // Respond to P2P Node Ping's
            parameters.TemplateBehaviors.FindOrCreate<PingPongBehavior>();
            parameters.TemplateBehaviors.Add(new SlimChainBehavior(Chain));
            parameters.TemplateBehaviors.Add(new AddressManagerBehavior(AddressManager));
            parameters.TemplateBehaviors.Add(new BroadcastHubBehavior());
            return parameters;
        }


        public void LoadChainData()
        {
            _chainLock.WaitOne();
            if (File.Exists(GetChainFile()))
                Chain.Load(File.OpenRead(GetChainFile()));
            _chainLock.ReleaseMutex();
        }

        public void SaveChainData()
        {
            if (Chain.Height != this._height)
            {
                _height = Chain.Height;
                //_logger.Debug("Chain Height: {0}", _height);
                _chainLock.WaitOne();
                using (FileStream fs = File.Open(GetChainFile(), FileMode.Create))
                {
                    Chain.Save(fs);
                    fs.Close();
                }
                _chainLock.ReleaseMutex();
            }
        }

        public async Task Connect()
        {
            await Task.Factory.StartNew(() =>
            {
                LoadChainData();
                _nodeGroup.AllowSameGroup = true;
                _nodeGroup.Requirements.SupportSPV = true;

                foreach (NetworkAddress networkAddress in BootstrapNodes)
                {
                    AddressManager.Add(networkAddress);
                }


                _nodeGroup.Connect();
                _nodeGroup.ConnectedNodes.Added += (s, e) =>
                {
                    var node = e.Node;
                    node.MessageReceived += HeadersMessageHandler.Process;
                    node.MessageReceived += NodeAddressMessageHandler.ProcessAsync;

                    node.MessageReceived += (node1, message) =>
                    {
                        //_logger.Debug("Command: {0}", message.Message.Command);
                        //_logger.Debug("Peers Connected: {0}", _nodeGroup.ConnectedNodes.Count);
                        SaveChainData();
                    };

                    node.Disconnected += n =>
                    {
                        // TrackerBehavior has probably disconnected the node because of too many false positives...
                        Console.WriteLine("Disconnected!");
                    };
                };
            });

        }

        public void Stop()
        {
            SaveChainData();
            if (_nodeGroup != null)
                _nodeGroup.Disconnect();
        }

        public void Dispose()
        {
            this.Stop();
        }
    }
}
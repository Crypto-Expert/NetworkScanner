using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Common;
using NBitcoin;
using NBitcoin.Protocol;
using NBitcoin.Protocol.Behaviors;
using NetworkScanner.CLI.Chains.MessageHandlers;
using NServiceBus;

namespace NetworkScanner.CLI.Chains
{
    public class BaseChain : IChain
    {
        public BaseChain()
        {
            LoadChainData();
            RegisterEventHandlers();
        }

        #region Chain Events
        public delegate void ChainEventHandler(BaseChain chain, Node sender, IncomingMessage message);
        event ChainEventHandler ChainEvent;

        public void RebroadcastEvent(Node node, IncomingMessage message)
        {
            ChainEvent(this, node, message);
        }

        public void RegisterEventHandlers()
        {
            ChainEvent += NodeAddressMessageHandler.Process;
            //ChainEvent += HeadersMessageHandler.Process;
        }

        #endregion

        #region Protected Vars
        protected AddressManager AddressManager = new AddressManager();
        protected virtual List<NetworkAddress> BootstrapNodes => new List<NetworkAddress>();
        protected SlimChain Chain => new SlimChain(Network.GenesisHash);
        protected Mutex _chainLock = new Mutex();
        protected IEndpointInstance EndpointInstance = null;
        protected readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        protected NodesGroup _nodeGroup = null;

        public virtual string ChainName => null;
        public virtual Network Network => null;
        public virtual string ChainFile => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), ChainName);

        #endregion

        #region Node Connection Behaviours
        public NodeConnectionParameters GetNodeConnectionParameters()
        {
            NodeConnectionParameters parameters = new NodeConnectionParameters();
            parameters.TemplateBehaviors.FindOrCreate<PingPongBehavior>();
            parameters.TemplateBehaviors.Add(new SlimChainBehavior(Chain));
            parameters.TemplateBehaviors.Add(new AddressManagerBehavior(AddressManager));
            parameters.TemplateBehaviors.Add(new BroadcastHubBehavior());
            return parameters;
        }
        #endregion

        #region Chain State
        public void LoadChainData()
        {
            _chainLock.WaitOne();
            if (File.Exists(ChainFile))
                Chain.Load(File.OpenRead(ChainFile));
            _chainLock.ReleaseMutex();
        }

        public void SaveChainData()
        {
                _chainLock.WaitOne();
                using (FileStream fs = File.Open(ChainFile, FileMode.Create))
                {
                    Chain.Save(fs);
                    fs.Close();
                }
                _chainLock.ReleaseMutex();
        }
        #endregion

        #region P2P Connection Management
        public async Task Connect()
        {
            await Task.Factory.StartNew(async () =>
            {
                EndpointInstance = await BusManager.StartServiceBus(Network.Name);
                _nodeGroup = new NodesGroup(Network, GetNodeConnectionParameters())
                {
                    AllowSameGroup = true,
                };
                _nodeGroup.Requirements.SupportSPV = true;

                foreach (NetworkAddress networkAddress in BootstrapNodes)
                {
                    AddressManager.Add(networkAddress);
                }

                _nodeGroup.Connect();
                _nodeGroup.ConnectedNodes.Added += (s, e) =>
                {
                    var node = e.Node;
                    node.MessageReceived += RebroadcastEvent;

                    node.MessageReceived += (node1, message) =>
                    {
                        Console.WriteLine("Message Received");
                        SaveChainData();
                    };

                    node.Disconnected += n =>
                    {
                        Console.WriteLine("Disconnected!");
                    };
                };
            });

        }

        public async Task StopAsync()
        {
            SaveChainData();
            if (_nodeGroup != null)
                _nodeGroup.Disconnect();

            if (EndpointInstance != null)
                await EndpointInstance.Stop();
        }

        #endregion

        #region Event Bus
        public async Task SendCommand(ICommand command)
        {
            await this.EndpointInstance.Send(command);
        }
        #endregion
        public async void Dispose() => await StopAsync();
    }
}

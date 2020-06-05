﻿using NBitcoin.Protocol;
using SystemBus;
using SystemBus.Commands;

namespace NetworkScanner.CLI.Chains.MessageHandlers
{
    public static class NodeAddressMessageHandler
    {
        public static void Process(BaseChain chain, Node node, IncomingMessage message)
        {
            if (message.Message.Command != "addr")
                return;

            AddrPayload payload = (AddrPayload)message.Message.Payload;

            NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
            foreach(NetworkAddress address in payload.Addresses)
            {
                AddNodeCommand command = new AddNodeCommand
                {
                    Address = address.Endpoint.Address.ToString(),
                    Port = address.Endpoint.Port,
                };
            }
            //_logger.Debug("Processing Address Message: {0}", payload.ToString());
        }
    }
}

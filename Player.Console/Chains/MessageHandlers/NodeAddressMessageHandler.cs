using NBitcoin.Protocol;
using Common.Commands;

namespace NetworkScanner.CLI.Chains.MessageHandlers
{
    public static class NodeAddressMessageHandler
    {
        public static void Process(BaseChain chain, Node node, IncomingMessage message)
        {
            if (message.Message.Command != "addr")
                return;

            NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
            AddrPayload payload = (AddrPayload)message.Message.Payload;

            foreach(NetworkAddress address in payload.Addresses)
            {
                AddNodeCommand command = new AddNodeCommand
                {
                    Chain = chain.ChainName,
                    Address = address.Endpoint.Address.ToString(),
                    Port = address.Endpoint.Port,
                };
                chain.SendCommand(command).GetAwaiter().GetResult();
            }
        }
    }
}

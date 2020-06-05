using System;
using NBitcoin.Protocol;

namespace NetworkScanner.CLI.Chains.MessageHandlers
{
    public static class HeadersMessageHandler
    {
        public static void Process(BaseChain chain, Node node, IncomingMessage message)
        {
            if (message.Message.Command != "headers")
                return;

            NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

            HeadersPayload payload = (HeadersPayload)message.Message.Payload;

            _logger.Debug("Processing Headers Message: {0}", payload.Headers.ToString());
        }
    }
}

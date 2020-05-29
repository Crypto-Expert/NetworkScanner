using System;
using NBitcoin.Protocol;

namespace NetworkScanner.CLI.Chains.MessageHandlers
{
    public static class HeadersMessageHandler
    {
        public static void Process(Node node, IncomingMessage message)
        {
            if (message.Message.Command != "headers")
                return;

            HeadersPayload payload = (HeadersPayload)message.Message.Payload;

            NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
            //_logger.Debug("Processing Headers Message: {0}", payload.Headers.ToString());
        }
    }
}

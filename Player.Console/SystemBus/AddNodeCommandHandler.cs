using System.Threading.Tasks;
using NServiceBus;
using SystemBus.Commands;

namespace NetworkScanner.SystemBus
{
    public class AddNodeCommandHandler : IHandleMessages<AddNodeCommand>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async Task Handle(AddNodeCommand message, IMessageHandlerContext context)
        {
            Logger.Info($"Received Add Node Command, Node = {message.Address}:{message.Port}");

        }
    }
}

using System.Threading.Tasks;
using NServiceBus;
using Common.Commands;
using NServiceBus.Logging;

namespace Peers.Handlers
{
    public class AddNodeCommandHandler : IHandleMessages<AddNodeCommand>
    {
        static ILog Logger = LogManager.GetLogger<AddNodeCommandHandler>();

        public async Task Handle(AddNodeCommand message, IMessageHandlerContext context)
        {
            Logger.Info($"Received Add Node Command, Node = {message.Address}:{message.Port}, Chain = {message.Chain}");
        }
    }
}

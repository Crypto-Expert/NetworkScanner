using NServiceBus;

namespace SystemBus.Commands
{
    public class AddNodeCommand : ICommand
    {
        public string Address { get; set; }
        public int Port { get; set; }
    }
}

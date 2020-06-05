using NServiceBus;

namespace Common.Commands
{
    public class AddNodeCommand : ICommand
    {
        public string Chain { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
    }
}

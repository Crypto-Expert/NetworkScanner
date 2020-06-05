using System.Threading.Tasks;
using NServiceBus;
using Common;

namespace Blocks
{
    class Program
    {
        static protected IEndpointInstance EndpointInstance = null;
        static protected NLog.Logger Logger = Common.Logger.GetLogger("Blocks");

        static async Task Main(string[] args)
        {
            EndpointInstance = await BusManager.StartServiceBus("Blocks");

        }

        public async Task StopAsync()
        {
            if (EndpointInstance != null)
                await EndpointInstance.Stop();
        }

        #region Event Bus
        public async Task SendCommand(ICommand command)
        {
            await EndpointInstance.Send(command);
        }
        #endregion

        public async void Dispose() => await StopAsync();
    }
}

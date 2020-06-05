using System.Threading.Tasks;
using Common;
using NServiceBus;

namespace Peers
{
    class Program
    {
        static protected IEndpointInstance EndpointInstance = null;
        static protected NLog.Logger Logger = Common.Logger.GetLogger("Peers");

        static async Task Main()
        {
            EndpointInstance = await BusManager.StartServiceBus("Peers");
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

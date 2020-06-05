using System.Threading.Tasks;
using NServiceBus;

namespace SystemBus
{
    public static class BusManager
    {
        public static async Task<IEndpointInstance> StartServiceBus(string name)
        {
            var endpointConfiguration = new EndpointConfiguration(name);
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            var EndpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            return EndpointInstance;

        }

        public static async Task StopServiceBus(IEndpointInstance endpointInstance)
        {
            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }

        public static async Task SendLocal(object command, IEndpointInstance endpointInstance)
        { 
            await endpointInstance.SendLocal(command)
                .ConfigureAwait(false);
        }
    }
}

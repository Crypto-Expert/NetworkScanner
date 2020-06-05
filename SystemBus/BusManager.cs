using System.Threading.Tasks;
using NServiceBus;
using Common.Commands;
using NServiceBus.Logging;

namespace Common
{
    public static class BusManager
    {
        public static async Task<IEndpointInstance> StartServiceBus(string name)
        {
            var endpointConfiguration = new EndpointConfiguration(name);
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(AddNodeCommand), "Peers");

            var EndpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);
            return EndpointInstance;

        }

        public static async Task StopServiceBus(IEndpointInstance endpointInstance)
        {
            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}

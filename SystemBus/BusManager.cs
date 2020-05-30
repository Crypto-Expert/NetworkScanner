using System;
using System.Threading.Tasks;
using NServiceBus;

namespace SystemBus
{
    public static class BusManager
    {
        private static IEndpointInstance EndpointInstance = null;

        public static IEndpointInstance GetServiceBus()
        {
            return EndpointInstance;
        }

        public static async Task StartServiceBus()
        {
            if (GetServiceBus() != null)
                return;

            var endpointConfiguration = new EndpointConfiguration("NetworkScanner");
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            EndpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

        }

        public static async Task StopServiceBus()
        {
            if (GetServiceBus() == null)
                return;

            await EndpointInstance.Stop()
                .ConfigureAwait(false);
        }

        public static async Task SendLocal(object command)
        {
            if (GetServiceBus() == null)
                return;

            await EndpointInstance.SendLocal(command)
                .ConfigureAwait(false);
        }
    }
}

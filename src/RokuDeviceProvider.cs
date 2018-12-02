using System.Threading;
using System.Threading.Tasks;
using RokuDotNet.Client;
using RokuDotNet.Proxy;
using RokuDotNet.Proxy.Mqtt;
using RokuDotNet.Rest;

namespace RokuDotNet.Rest.Service
{
    internal sealed class RokuDeviceProvider : IRokuDeviceProvider
    {
        private readonly IRokuRpcClient rpcClient;

        public RokuDeviceProvider(string connectionString)
        {
            this.rpcClient = new IoTHubRokuRpcClient(connectionString);
        }

        #region IRokuDeviceProvider Members

        public Task<IRokuDevice> GetDeviceFromIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var device = new ProxyRokuDevice(id, this.rpcClient);

            return Task.FromResult<IRokuDevice>(device);
        }

        #endregion
    }
}
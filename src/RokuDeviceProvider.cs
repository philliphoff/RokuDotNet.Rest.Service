using System.Threading;
using System.Threading.Tasks;
using RokuDotNet.Client;
using RokuDotNet.Proxy;
using RokuDotNet.Proxy.Mqtt;
using RokuDotNet.Rest;
using Serilog;

namespace RokuDotNet.Rest.Service
{
    internal sealed class RokuDeviceProvider : IRokuDeviceProvider
    {
        private readonly IRokuRpcClient rpcClient;

        public RokuDeviceProvider(string connectionString, ILogger logger)
        {
            this.rpcClient =
                new RpcLoggingHandler(
                    new IoTHubRokuRpcClient(connectionString),
                    logger);
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
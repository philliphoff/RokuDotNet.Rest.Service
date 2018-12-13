using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using RokuDotNet.Proxy;
using Serilog;

namespace RokuDotNet.Rest.Service
{
    internal sealed class RpcLoggingHandler : IRokuRpcClient
    {
        private readonly IRokuRpcClient innerClient;
        private readonly ILogger logger;

        public RpcLoggingHandler(IRokuRpcClient innerClient, ILogger logger)
        {
            this.innerClient = innerClient ?? throw new ArgumentNullException(nameof(innerClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region IRokuRpcClient Members

        public async Task<TMethodResponsePayload> InvokeMethodAsync<TMethodPayload, TMethodResponsePayload>(string deviceId, string methodName, TMethodPayload payload, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.logger.Information("Sending RPC {MethodName} for device {DeviceId}", methodName, deviceId);

            var watch = new Stopwatch();

            watch.Start();

            var response = await this.innerClient.InvokeMethodAsync<TMethodPayload, TMethodResponsePayload>(deviceId, methodName, payload, cancellationToken).ConfigureAwait(false);

            watch.Stop();

            this.logger.Information("Received RPC {Response} in {Milliseconds}ms", response, watch.ElapsedMilliseconds);

            return response;
        }

        #endregion
    }
}
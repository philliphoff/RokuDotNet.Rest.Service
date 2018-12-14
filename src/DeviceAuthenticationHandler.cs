using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RokuDotNet.Rest.Service
{
    internal sealed class DeviceAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public DeviceAuthenticationSchemeOptions()
        {
        }

        public string DeviceKey { get; set; }
    }

    internal sealed class DeviceAuthenticationHandler : AuthenticationHandler<DeviceAuthenticationSchemeOptions>
    {
        public DeviceAuthenticationHandler(
            IOptionsMonitor<DeviceAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (this.Request.Headers.ContainsKey("Authorization"))
            {
                if(AuthenticationHeaderValue.TryParse(this.Request.Headers["Authorization"], out AuthenticationHeaderValue headerValue))
                {
                    if (StringComparer.OrdinalIgnoreCase.Equals(headerValue.Scheme, "Device"))
                    {
                        if (StringComparer.Ordinal.Equals(headerValue.Parameter, this.Options.DeviceKey))
                        {
                            var identity = new ClaimsIdentity(Enumerable.Empty<Claim>(), Scheme.Name);
                            var principal = new ClaimsPrincipal(identity);
                            var ticket = new AuthenticationTicket(principal, Scheme.Name);

                            return Task.FromResult(AuthenticateResult.Success(ticket));
                        }
                    }
                }
            }

            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }
}
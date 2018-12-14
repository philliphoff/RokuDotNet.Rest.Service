using RokuDotNet.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace RokuDotNet.Rest.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable("ROKU_REST_SERVICE_CONNECTIONSTRING");
            var deviceKey = Environment.GetEnvironmentVariable("ROKU_REST_SERVICE_DEVICEKEY");

            services.AddSingleton<ILogger>(
                _ =>
                    new LoggerConfiguration()
                        .WriteTo
                        .Console()
                        .CreateLogger());

            services.AddSingleton<IRokuDeviceProvider>(serviceProvider => new RokuDeviceProvider(connectionString, serviceProvider.GetService<ILogger>()));

            services.AddAuthentication("Device")
                .AddScheme<DeviceAuthenticationSchemeOptions, DeviceAuthenticationHandler>(
                    "Device",
                    options =>
                    {
                        options.DeviceKey = deviceKey; 
                    });

            services
                .AddMvc(
                    options =>
                    {
                        var policy = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .Build();
                        options.Filters.Add(new AuthorizeFilter(policy));
                    }
                )
                .AddXmlSerializerFormatters()
                .AddApplicationPart(typeof(RokuController).GetType().Assembly)
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}

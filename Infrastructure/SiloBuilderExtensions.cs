using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using Orleans.Hosting;
using RockPaperOrleans;
using System.Net;

namespace Microsoft.Extensions.Hosting
{
    public static class SiloBuilderExtensions
    {
        public static ISiloBuilder EnlistPlayer<TPlayer>(this ISiloBuilder builder) 
            where TPlayer : PlayerBase
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<TPlayer>();
                services.AddHostedService<PlayerWorkerBase<TPlayer>>();
            });

            return builder;
        }

        public static ISiloBuilder PlayRockPaperOrleans(this ISiloBuilder builder, IConfiguration configuration)
        {
            builder
                .CreateOrConnectToGameCluster(configuration)
                    .UseAzureStorageClustering()
                    .UseAzureStorageGrainStorage();

            return builder;
        }

        private static IAzureSiloBuilder? CreateOrConnectToGameCluster(this ISiloBuilder builder, IConfiguration configuration)
        {
            builder
                .Configure<SiloOptions>(options =>
                {
                    options.SiloName = string.Format(configuration.GetValue<string>(
                        OrleansOnAzureConfiguration.EnvironmentVariableNames.SiloNameTemplate
                    ), Environment.MachineName);
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = configuration.GetValue<string>(
                        OrleansOnAzureConfiguration.EnvironmentVariableNames.ClusterId
                    );
                    options.ServiceId = configuration.GetValue<string>(
                        OrleansOnAzureConfiguration.EnvironmentVariableNames.ServiceId
                    );
                })
                .Configure<EndpointOptions>(options =>
                {
                    options.AdvertisedIPAddress = IPAddress.Loopback;
                    options.SiloPort = configuration.GetValue<int>(
                        OrleansOnAzureConfiguration.EnvironmentVariableNames.SiloPort
                    );
                    options.GatewayPort = configuration.GetValue<int>(
                        OrleansOnAzureConfiguration.EnvironmentVariableNames.GatewayPort
                    );
                });

            IAzureSiloBuilder? result = null;

            builder.ConfigureServices((ctx, services) =>
            {
                result = new AzureSiloBuilder(builder, ctx.Configuration);
                services.AddSingleton<IAzureSiloBuilder>(result);
            });

            return result;
        }
    }
}
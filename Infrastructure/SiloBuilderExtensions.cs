using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Clustering.CosmosDB;
using Orleans.Configuration;
using Orleans.Persistence.CosmosDB;
using System.Net;

namespace Orleans.Hosting
{
    public static class SiloBuilderExtensions
    {
        public static IAzureSiloBuilder HostInAzure(this ISiloBuilder builder)
        {
            builder
                .Configure<SiloOptions>(options =>
                {
                    options.SiloName = "Silo";
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "RockPaperOrleans";
                })
                .Configure<EndpointOptions>(options =>
                {
                    options.AdvertisedIPAddress = IPAddress.Loopback;
                    options.SiloPort = 11111;
                    options.GatewayPort = 30000;
                });

            IAzureSiloBuilder result = null;

            builder.ConfigureServices((ctx, services) =>
            {
                result = new AzureSiloBuilder(builder, ctx.Configuration);
                services.AddSingleton<IAzureSiloBuilder>(result);
            });

            return result;
        }
    }

    public interface IAzureSiloBuilder
    {
        IAzureSiloBuilder UseCosmosDbClustering();
        IAzureSiloBuilder UseCosmosDbGrainStorage();
    }

    public class AzureSiloBuilder : IAzureSiloBuilder
    {
        private ISiloBuilder SiloBuilder;
        private IConfiguration Configuration;

        public AzureSiloBuilder(ISiloBuilder builder, IConfiguration configuration)
        {
            SiloBuilder = builder;
            Configuration = configuration;
        }

        public IAzureSiloBuilder UseCosmosDbClustering()
        {
            SiloBuilder.UseCosmosDBMembership((CosmosDBClusteringOptions clusteringOptions) =>
            {
                clusteringOptions.ConnectionMode = Microsoft.Azure.Cosmos.ConnectionMode.Direct;
                clusteringOptions.AccountEndpoint =Configuration.GetValue<string>("AccountEndpoint");
                clusteringOptions.AccountKey = Configuration.GetValue<string>("AccountKey");
                clusteringOptions.DB = Configuration.GetValue<string>("DB");
                clusteringOptions.CanCreateResources = true;
            });

            return this;
        }

        public IAzureSiloBuilder UseCosmosDbGrainStorage()
        {
            SiloBuilder.AddCosmosDBGrainStorageAsDefault((CosmosDBStorageOptions cosmosOptions) =>
            {
                cosmosOptions.AccountEndpoint = Configuration.GetValue<string>("AccountEndpoint");
                cosmosOptions.AccountKey = Configuration.GetValue<string>("AccountKey");
                cosmosOptions.DB = Configuration.GetValue<string>("DB");
                cosmosOptions.CanCreateResources = true;
            });

            return this;
        }
    }
}
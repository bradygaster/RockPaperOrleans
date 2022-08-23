using Microsoft.Extensions.Configuration;
using Orleans.Clustering.AzureStorage;
using Orleans.Clustering.CosmosDB;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Persistence.CosmosDB;

namespace RockPaperOrleans
{
    public interface IAzureSiloBuilder
    {
        IAzureSiloBuilder UseCosmosDbClustering();
        IAzureSiloBuilder UseCosmosDbGrainStorage();
        IAzureSiloBuilder UseAzureStorageClustering();
        IAzureSiloBuilder UseAzureStorageGrainStorage();
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
                clusteringOptions.AccountEndpoint = Configuration.GetValue<string>(
                    OrleansOnAzureConfiguration.EnvironmentVariableNames.CosmosDbAccountEndpoint
                    );
                clusteringOptions.AccountKey = Configuration.GetValue<string>(
                    OrleansOnAzureConfiguration.EnvironmentVariableNames.CosmosDbAccountKey
                    );
                clusteringOptions.DB = Configuration.GetValue<string>(
                    OrleansOnAzureConfiguration.EnvironmentVariableNames.CosmosDbDatabase
                    );
                clusteringOptions.CanCreateResources = true;
            });

            return this;
        }

        public IAzureSiloBuilder UseCosmosDbGrainStorage()
        {
            SiloBuilder.AddCosmosDBGrainStorageAsDefault((CosmosDBStorageOptions cosmosOptions) =>
            {
                cosmosOptions.AccountEndpoint = Configuration.GetValue<string>(
                    OrleansOnAzureConfiguration.EnvironmentVariableNames.CosmosDbAccountEndpoint
                    );
                cosmosOptions.AccountKey = Configuration.GetValue<string>(
                    OrleansOnAzureConfiguration.EnvironmentVariableNames.CosmosDbAccountKey
                    );
                cosmosOptions.DB = Configuration.GetValue<string>(
                    OrleansOnAzureConfiguration.EnvironmentVariableNames.CosmosDbDatabase
                    );
                cosmosOptions.CanCreateResources = true;
            });

            return this;
        }

        public IAzureSiloBuilder UseAzureStorageClustering()
        {
            SiloBuilder.UseAzureStorageClustering((AzureStorageClusteringOptions options) =>
            {
                options.ConfigureTableServiceClient(Configuration.GetValue<string>(
                    OrleansOnAzureConfiguration.EnvironmentVariableNames.AzureStorageConnectionString
                    ));
            });

            return this;
        }

        public IAzureSiloBuilder UseAzureStorageGrainStorage()
        {
            SiloBuilder.AddAzureTableGrainStorageAsDefault((AzureTableStorageOptions options) =>
            {
                options.ConfigureTableServiceClient(Configuration.GetValue<string>(
                    OrleansOnAzureConfiguration.EnvironmentVariableNames.AzureStorageConnectionString
                    ));

                options.UseJson = true;
                options.IndentJson = true;
            });

            return this;
        }
    }
}

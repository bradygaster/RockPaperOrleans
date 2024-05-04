using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.Hosting;

public static class SiloBuilderExtensions
{
    public static WebApplicationBuilder AddRockPaperOrleans(this WebApplicationBuilder builder, Action<ISiloBuilder>? action = null)
    {
        builder.AddKeyedAzureTableClient("clustering");
        builder.AddKeyedAzureBlobClient("grainstorage");
        builder.UseOrleans(siloBuilder =>
        {
            if(action != null)
            {
                action(siloBuilder);
            }
        });

        return builder;
    }

    public static ISiloBuilder AddPlayer<TPlayer>(this ISiloBuilder builder) where TPlayer : IPlayerGrain
    {
        builder.ConfigureServices(services =>
        {
            services.AddHostedService<PlayerWorkerBase<TPlayer>>();
        });

        return builder;
    }
}
namespace Microsoft.Extensions.Hosting;

public static class SiloBuilderExtensions
{
    public static WebApplicationBuilder AddRockPaperOrleans(this WebApplicationBuilder builder, Action<ISiloBuilder>? action = null)
    {
        builder.AddKeyedAzureTableClient("clustering", _ => _.DisableTracing = true);
        builder.AddKeyedAzureBlobClient("grainstorage", _ => _.DisableTracing = true);
        builder.UseOrleans(siloBuilder =>
        {
            action?.Invoke(siloBuilder);
        });

        return builder;
    }

    public static ISiloBuilder AddPlayer<TPlayer>(this ISiloBuilder builder) where TPlayer : IPlayerGrain
    {
        builder.ConfigureServices(services =>
        {
            services.AddHostedService<PlayerWorker<TPlayer>>();
        });

        return builder;
    }
}
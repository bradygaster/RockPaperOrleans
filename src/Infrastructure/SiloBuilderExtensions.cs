namespace Microsoft.Extensions.Hosting;

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
}
using Orleans;
using Orleans.Hosting;
using Paper;
using RockPaperOrleans.Abstractions;

IHost host = Host.CreateDefaultBuilder(args)
    .UseOrleans((context, siloBuilder) =>
    {
        siloBuilder
            .UseDashboard(dashboardOptions => dashboardOptions.HostSelf = false)
            .HostInAzure(context.Configuration)
                .UseCosmosDbClustering()
                .UseCosmosDbGrainStorage();
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<IPlayerObserver, PaperPlayer>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();

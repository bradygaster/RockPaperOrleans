using Orleans;
using Orleans.Hosting;
using Rock;
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
        services.AddSingleton<IPlayerObserver, RockPlayer>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();

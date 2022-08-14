using Orleans;
using Orleans.Hosting;
using RockPaperOrleans.Abstractions;
using Scissors;

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
        services.AddSingleton<IPlayerObserver, ScissorsPlayer>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();

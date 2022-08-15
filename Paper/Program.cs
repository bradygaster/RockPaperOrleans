using Orleans;
using Orleans.Hosting;
using RockPaperOrleans;
using RockPaperOrleans.Abstractions;

await Task.Delay(30000); // for debugging, give the silo time to warm up

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
        services.AddSingleton<AlwaysPaper>();
        services.AddHostedService<AlwaysPaperWorker>();
    })
    .Build();

await host.RunAsync();

public class AlwaysPaper : PlayerBase
{
    public AlwaysPaper(ILogger<AlwaysPaper> logger) : base(logger) { }

    public override Task<Play> Go()
    {
        return Task.FromResult(Play.Paper);
    }
}

public class AlwaysPaperWorker : PlayerWorkerBase<AlwaysPaper>
{
    public AlwaysPaperWorker(AlwaysPaper playerObserver, IGrainFactory grainFactory) 
        : base(playerObserver, grainFactory)
    {
    }
}
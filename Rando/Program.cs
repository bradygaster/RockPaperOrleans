using Orleans;
using Orleans.Hosting;
using RockPaperOrleans.Abstractions;
using RockPaperOrleans;
using System.ComponentModel;

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
        services.AddSingleton<Rando>();
        services.AddHostedService<RandoWorker>();
    })
    .Build();

await host.RunAsync();

public class Rando : PlayerBase
{
    public Rando(ILogger<Rando> logger) : base(logger) { }  

    Play LastPlay = Play.Rock;

    public override Task<Play> Go()
    {
        LastPlay = (Play)Random.Shared.Next(0, 3);
        Logger.LogInformation($"Rando throws {LastPlay}.");
        return Task.FromResult(LastPlay);
    }
}

public class RandoWorker : PlayerWorkerBase<Rando>
{
    public RandoWorker(Rando playerObserver, IGrainFactory grainFactory)
        : base(playerObserver, grainFactory)
    {
    }
}
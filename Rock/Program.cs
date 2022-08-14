using Orleans;
using Orleans.Hosting;
using RockPaperOrleans.Abstractions;
using RockPaperOrleans;

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
        services.AddSingleton<AlwaysRock>();
        services.AddHostedService<AlwaysRockWorker>();
    })
    .Build();

await host.RunAsync();

public class AlwaysRock : PlayerBase
{
    public ILogger<AlwaysRock> Logger { get; set; }

    public AlwaysRock(ILogger<AlwaysRock> logger)
        => Logger = logger;

    public override Task<Play> Go()
    {
        return Task.FromResult(Play.Rock);
    }

    public override Task OnGameWon(Player player)
    {
        Logger.LogInformation("Rock breaks scissors.");
        return base.OnGameWon(player);
    }
}

public class AlwaysRockWorker : PlayerWorkerBase<AlwaysRock>
{
    public AlwaysRockWorker(AlwaysRock playerObserver, IGrainFactory grainFactory)
        : base(playerObserver, grainFactory)
    {
    }
}
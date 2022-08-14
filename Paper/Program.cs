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
    public ILogger<AlwaysPaper> Logger { get; set; }

    public AlwaysPaper(ILogger<AlwaysPaper> logger)
        => Logger = logger;

    public override Task<Play> Go()
    {
        return Task.FromResult(Play.Paper);
    }

    public override Task OnGameWon(Player player)
    {
        Logger.LogInformation("Paper covers rock.");
        return base.OnGameWon(player);
    }

    public override Task OnOpponentSelected(Player opponent)
    {
        Logger.LogInformation($"{GetType().Name} is about to play {opponent.Name}.");
        return base.OnOpponentSelected(opponent);
    }

    public override Task OnGameLost(Player player)
    {
        Logger.LogInformation($"{GetType().Name} loses to {Opponent.Name}.");
        return base.OnGameLost(player);
    }

    public override Task OnGameTied(Player player)
    {
        Logger.LogInformation($"{GetType().Name} ties with {Opponent.Name}.");
        return base.OnGameTied(player);
    }
}

public class AlwaysPaperWorker : PlayerWorkerBase<AlwaysPaper>
{
    public AlwaysPaperWorker(AlwaysPaper playerObserver, IGrainFactory grainFactory) 
        : base(playerObserver, grainFactory)
    {
    }
}
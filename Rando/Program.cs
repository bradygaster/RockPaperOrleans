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
    public ILogger<Rando> Logger { get; set; }

    public Rando(ILogger<Rando> logger)
        => Logger = logger;

    Play LastPlay = Play.Rock;

    public override Task<Play> Go()
    {
        LastPlay = (Play)Random.Shared.Next(0, 3);
        Logger.LogInformation($"Rando throws {LastPlay}.");
        return Task.FromResult(LastPlay);
    }

    public override Task OnGameWon(Player player)
    {
        Logger.LogInformation($"{GetType().Name} wins against {Opponent.Name}.");
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

public class RandoWorker : PlayerWorkerBase<Rando>
{
    public RandoWorker(Rando playerObserver, IGrainFactory grainFactory)
        : base(playerObserver, grainFactory)
    {
    }
}
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
        services.AddSingleton<AlwaysScissors>();
        services.AddHostedService<AlwaysScissorsWorker>();
    })
    .Build();

await host.RunAsync();

public class AlwaysScissors : PlayerBase
{
    public ILogger<AlwaysScissors> Logger { get; set; }

    public AlwaysScissors(ILogger<AlwaysScissors> logger)
        => Logger = logger;

    public override Task<Play> Go()
    {
        return Task.FromResult(Play.Scissors);
    }

    public override Task OnGameWon(Player player)
    {
        Logger.LogInformation("Scissors cut paper.");
        return base.OnGameWon(player);
    }
}

public class AlwaysScissorsWorker : PlayerWorkerBase<AlwaysScissors>
{
    public AlwaysScissorsWorker(AlwaysScissors playerObserver, IGrainFactory grainFactory)
        : base(playerObserver, grainFactory)
    {
    }
}
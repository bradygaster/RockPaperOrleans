using Orleans;
using Orleans.Hosting;
using RockPaperOrleans.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans((context, siloBuilder) =>
{
    siloBuilder
        .UseDashboard(dashboardOptions => dashboardOptions.HostSelf = false)
        .HostInAzure(context.Configuration)
            .UseCosmosDbClustering()
            .UseCosmosDbGrainStorage();
});

builder.Services.AddServicesForSelfHostedDashboard();
builder.Services.AddHostedService<GameEngine>();

var app = builder.Build();
app.UseOrleansDashboard();

app.Run();

// the game engine will host the game engine grain
public class GameEngine : BackgroundService
{
    public GameEngine(IGrainFactory grainFactory, ILogger<GameEngine> logger)
    {
        GrainFactory = grainFactory;
        Logger = logger;
        GameEngineGrain = GrainFactory.GetGrain<IGameEngineGrain>(Guid.Empty);
        MatchmakingGrain = GrainFactory.GetGrain<IMatchmakingGrain>(Guid.Empty);
    }

    public IGrainFactory GrainFactory { get; set; }
    public ILogger<GameEngine> Logger { get; set; }
    public IGameEngineGrain GameEngineGrain { get; }
    public IMatchmakingGrain MatchmakingGrain { get; private set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if(await GameEngineGrain.IsGameComplete())
            {
                await GameEngineGrain.StartNewGame();
            }
            else
            {
                var currentGame = await GameEngineGrain.CurrentGame();
                var gameGrain = GrainFactory.GetGrain<IGameGrain>(currentGame.Id);

                if(currentGame.Player1 == null && currentGame.Player2 == null)
                {
                    await gameGrain.SelectPlayers();
                }
                else
                {

                }
            }

            await Task.Delay(1000);
        }
    }
}

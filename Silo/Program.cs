using Orleans;
using Orleans.Hosting;
using RockPaperOrleans.Abstractions;
using RockPaperOrleans.Grains;

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
    }

    public IGrainFactory GrainFactory { get; set; }
    public ILogger<GameEngine> Logger { get; set; }
    public IGameGrain CurrentGameGrain { get; set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if(CurrentGameGrain == null)
            {
                CurrentGameGrain = GrainFactory.GetGrain<IGameGrain>(Guid.NewGuid());
            }

            var currentGame = await CurrentGameGrain.GetGame();

            if (currentGame.Player1 == null && currentGame.Player2 == null)
            {
                await CurrentGameGrain.SelectPlayers();
            }
            else
            {
                if(currentGame.Rounds > currentGame.Turns.Count)
                {
                    Logger.LogInformation("Players go here.");
                }
                else
                {
                    Logger.LogInformation("Score the game here.");
                }
            }

            await Task.Delay(5000);
        }
    }
}

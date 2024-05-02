using Orleans.Runtime;

namespace GameController;

public class GameEngine : BackgroundService
{
    public GameEngine(IGrainFactory grainFactory, ILogger<GameEngine> logger)
    {
        GrainFactory = grainFactory;
        Logger = logger;
    }

    public IGrainFactory GrainFactory { get; set; }
    public ILogger<GameEngine> Logger { get; set; }
    public IGameGrain? CurrentGameGrain { get; set; }
    public ILeaderboardGrain? LeaderboardGrain { get; set; }
    public DateTime DateStarted { get; private set; }
    public int GamesCompleted { get; set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var StartNewGame = () =>
        {
            CurrentGameGrain = GrainFactory.GetGrain<IGameGrain>(Guid.NewGuid());
            GamesCompleted += 1;
        };

        var UpdateSystemStatus = async (SystemStatusUpdate update) => await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).UpdateSystemStatus(update);
        var delay = 250;

        while (!stoppingToken.IsCancellationRequested)
        {
            // start a new game if we don't have one yet
            if (CurrentGameGrain == null) StartNewGame();

            var currentGame = await CurrentGameGrain.GetGame();

            // select players if they're unselected so far
            if (currentGame.Player1 == null && currentGame.Player2 == null)
            {
                await CurrentGameGrain.SelectPlayers();
            }
            else
            {
                if (currentGame.Rounds > currentGame.Turns.Count)
                {
                    await CurrentGameGrain.Go();
                    await Task.Delay(delay);
                    await CurrentGameGrain.ScoreTurn();
                }
                else
                {
                    await CurrentGameGrain.ScoreGame();
                    await Task.Delay(delay);
                    StartNewGame();
                }
            }

            // send a system status update
            var grainCount = await GrainFactory.GetGrain<IManagementGrain>(0).GetTotalActivationCount();
            await UpdateSystemStatus(new SystemStatusUpdate
            {
                DateStarted = DateStarted,
                GamesCompleted = GamesCompleted,
                TimeUp = DateStarted - DateTime.Now,
                GrainsActive = grainCount
            });

            await Task.Delay(delay);
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        DateStarted = DateTime.Now;
        return base.StartAsync(cancellationToken);
    }
}

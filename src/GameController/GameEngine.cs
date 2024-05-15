using Orleans.Runtime;

namespace GameController;

public class GameEngine(IGrainFactory grainFactory, ILogger<GameEngine> logger, GameEngineStateController gameEngineStateController) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var currentGameGrain = grainFactory.GetGrain<IGameGrain>(Guid.NewGuid());

        while (!stoppingToken.IsCancellationRequested)
        {
            if (gameEngineStateController.IsStarted)
            {
                try
                {
                    var currentGame = await currentGameGrain.GetGame();

                    // Select players if they're unselected so far
                    if (currentGame.Player1 is null || currentGame.Player2 is null)
                    {
                        await currentGameGrain.SelectPlayers();
                    }
                    else
                    {
                        if (currentGame.Rounds > currentGame.Turns.Count)
                        {
                            await currentGameGrain.Go();
                            await currentGameGrain.ScoreTurn();
                        }
                        else
                        {
                            await currentGameGrain.ScoreGame();
                            gameEngineStateController.IncrementGameCount();

                            // Start a new game.
                            currentGameGrain = grainFactory.GetGrain<IGameGrain>(Guid.NewGuid());
                        }
                    }

                    // Send a system status update
                    var grainCount = await grainFactory.GetGrain<IManagementGrain>(0).GetTotalActivationCount();
                    var leaderboardGrain = grainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
                    await leaderboardGrain.UpdateSystemStatus(new SystemStatusUpdate
                    {
                        DateStarted = gameEngineStateController.ServerStartedTime,
                        GamesCompleted = gameEngineStateController.GamesCompleted,
                        TimeUp = gameEngineStateController.Uptime.Elapsed,
                        GrainsActive = grainCount
                    });
                }
                catch (Exception ex)
                {
                    if (!stoppingToken.IsCancellationRequested)
                    {
                        logger.LogError(ex, "RPO: GameEngine error.");
                    }
                }
            }

            await Task.Delay(gameEngineStateController.GameLoopDelayInMilliseconds, stoppingToken);
        }
    }
}

using Orleans.Runtime;
using System.Diagnostics;

namespace GameController;

public class GameEngine(IGrainFactory grainFactory, ILogger<GameEngine> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var dateStarted = DateTime.UtcNow;
        var uptimeStopwatch = Stopwatch.StartNew();
        var gamesCompleted = 0;
        var currentGameGrain = grainFactory.GetGrain<IGameGrain>(Guid.NewGuid());
        var delay = TimeSpan.FromMilliseconds(100);
        while (!stoppingToken.IsCancellationRequested)
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
                        await Task.Delay(delay, stoppingToken);
                        await currentGameGrain.ScoreTurn();
                    }
                    else
                    {
                        await currentGameGrain.ScoreGame();
                        await Task.Delay(delay, stoppingToken);
                        ++gamesCompleted;

                        // Start a new game.
                        currentGameGrain = grainFactory.GetGrain<IGameGrain>(Guid.NewGuid());
                    }
                }

                // Send a system status update
                var grainCount = await grainFactory.GetGrain<IManagementGrain>(0).GetTotalActivationCount();
                var leaderboardGrain = grainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
                await leaderboardGrain.UpdateSystemStatus(new SystemStatusUpdate
                {
                    DateStarted = dateStarted,
                    GamesCompleted = gamesCompleted,
                    TimeUp = uptimeStopwatch.Elapsed,
                    GrainsActive = grainCount
                });

                await Task.Delay(delay, stoppingToken);
            }
            catch (Exception ex)
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogError(ex, "RPO: GameEngine error.");
                    await Task.Delay(delay, stoppingToken);
                }
            }
        }
    }
}

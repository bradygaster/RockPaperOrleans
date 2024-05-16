namespace Leaderboard;

public class LeaderboardObserverWorker(
    ILeaderboardGrainObserver leaderboardObserver,
    IGrainFactory grainFactory,
    ILogger<LeaderboardObserverWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var leaderboard = grainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
        var reference = grainFactory.CreateObjectReference<ILeaderboardGrainObserver>(leaderboardObserver);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await leaderboard.Subscribe(reference);
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (Exception ex)
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogError(ex, "RPO: LeaderboardObserverWorker error.");
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
        }

        await leaderboard.UnSubscribe(reference);
    }
}

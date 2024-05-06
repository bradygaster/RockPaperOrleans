namespace RockPaperOrleans;

public sealed class PlayerWorker<TPlayer>(IGrainFactory grainFactory, ILogger<PlayerWorker<TPlayer>> logger) 
    : BackgroundService where TPlayer : IPlayerGrain
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var playerSessionGrain = grainFactory.GetGrain<IPlayerSessionGrain>(typeof(TPlayer).Name);
        var playerGrain = grainFactory.GetGrain<IPlayerGrain>(typeof(TPlayer).Name, grainClassNamePrefix: typeof(TPlayer).Name);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await playerSessionGrain.SignIn(playerGrain).ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (Exception ex)
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogError(ex, "Error registering player {PlayerType}.", typeof(TPlayer));
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }

        if (playerSessionGrain is not null)
        {
            await playerSessionGrain.SignOut().ConfigureAwait(false);
        }
    }
}

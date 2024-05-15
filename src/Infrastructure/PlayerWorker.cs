namespace RockPaperOrleans;

public sealed class PlayerWorker<TPlayer>(IGrainFactory grainFactory, ILogger<PlayerWorker<TPlayer>> logger)
    : IHostedService where TPlayer : IPlayerGrain
{
    private IPlayerSessionGrain playerSessionGrain;
    private IPlayerGrain playerGrain;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        bool keepTrying = true;

        while(keepTrying)
        {
            try
            {
                playerSessionGrain = grainFactory.GetGrain<IPlayerSessionGrain>(typeof(TPlayer).Name);
                playerGrain = grainFactory.GetGrain<IPlayerGrain>(typeof(TPlayer).Name, grainClassNamePrefix: typeof(TPlayer).Name);
                await playerSessionGrain.SignIn(playerGrain).ConfigureAwait(false);
                keepTrying = false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error registering player {PlayerType}.", typeof(TPlayer));
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (playerSessionGrain is not null)
        {
            await playerSessionGrain.SignOut().ConfigureAwait(false);
        }
    }
}

namespace RockPaperOrleans;

public sealed class PlayerWorker<TPlayer>(IGrainFactory grainFactory, ILogger<PlayerWorker<TPlayer>> logger) 
    : IHostedService where TPlayer : IPlayerGrain
{
    public IPlayerSessionGrain? PlayerSessionGrain { get; set; }
    public IPlayerGrain? PlayerGrain { get; set; }
    public IGrainFactory GrainFactory { get; set; } = grainFactory;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            try
            {
                PlayerSessionGrain = GrainFactory.GetGrain<IPlayerSessionGrain>(typeof(TPlayer).Name);
                PlayerGrain = GrainFactory.GetGrain<IPlayerGrain>(typeof(TPlayer).Name, grainClassNamePrefix: typeof(TPlayer).Name);
                await PlayerSessionGrain.SignIn(PlayerGrain).ConfigureAwait(false);
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error registering player {PlayerType}.", typeof(TPlayer));
                await Task.Delay(1000, cancellationToken);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (PlayerSessionGrain is not null)
        {
            await PlayerSessionGrain.SignOut().ConfigureAwait(false);
        }
    }
}

using System.Threading;

namespace RockPaperOrleans;

public sealed class PlayerWorker<TPlayer>(IGrainFactory grainFactory, ILogger<PlayerWorker<TPlayer>> logger)
    : BackgroundService where TPlayer : IPlayerGrain
{
    private IPlayerSessionGrain playerSessionGrain;
    private IPlayerGrain playerGrain;

    private async Task SignPlayerIn(CancellationToken cancellationToken)
    {
        bool keepTrying = true;

        while (keepTrying)
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
                logger.LogWarning(ex, "Error registering player {PlayerType}. Game Engine may not be up yet. Trying again in 1 second.", typeof(TPlayer));
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (playerSessionGrain == null)
            {
                await SignPlayerIn(stoppingToken);
            }
            else
            {
                var isPlayerOnline = await playerSessionGrain.IsPlayerOnline();

                if (!isPlayerOnline)
                {
                    await SignPlayerIn(stoppingToken);
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }
}

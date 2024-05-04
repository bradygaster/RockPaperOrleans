using Microsoft.Extensions.Logging;

namespace RockPaperOrleans;

public class PlayerWorkerBase<TPlayer>(IGrainFactory grainFactory) 
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
                await PlayerSessionGrain.SignIn(PlayerGrain);
                return;
            }
            catch (Exception ex)
            {
                await Task.Delay(1000);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await PlayerSessionGrain.SignOut();
    }
}

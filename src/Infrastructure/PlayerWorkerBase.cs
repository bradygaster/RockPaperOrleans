using Microsoft.Extensions.Logging;

namespace RockPaperOrleans;

public class PlayerWorkerBase<TPlayer>(TPlayer playerObserver,
        IGrainFactory grainFactory) : IHostedService where TPlayer : PlayerBase
{
    public IPlayerGrain? PlayerGrain { get; set; }
    public TPlayer PlayerObserver { get; } = playerObserver;
    public IGrainFactory GrainFactory { get; set; } = grainFactory;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            try
            {
                PlayerGrain = GrainFactory.GetGrain<IPlayerGrain>(typeof(TPlayer).Name);
                var reference = GrainFactory.CreateObjectReference<IPlayerObserver>(PlayerObserver);
                await PlayerGrain.SignIn(reference);
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
        await PlayerGrain.SignOut();
    }
}

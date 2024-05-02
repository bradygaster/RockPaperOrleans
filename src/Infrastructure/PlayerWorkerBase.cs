using Microsoft.Extensions.Hosting;
using Orleans;
using RockPaperOrleans.Abstractions;

namespace RockPaperOrleans
{
    public class PlayerWorkerBase<TPlayer> : IHostedService where TPlayer : PlayerBase
    {
        public IPlayerGrain? PlayerGrain { get; set; }
        public TPlayer PlayerObserver { get; }
        public IGrainFactory GrainFactory { get; set; }

        public PlayerWorkerBase(TPlayer playerObserver,
            IGrainFactory grainFactory)
        {
            PlayerObserver = playerObserver;
            GrainFactory = grainFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            PlayerGrain = GrainFactory.GetGrain<IPlayerGrain>(typeof(TPlayer).Name);
            var reference = GrainFactory.CreateObjectReference<IPlayerObserver>(PlayerObserver);
            await PlayerGrain.SignIn(reference);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await PlayerGrain.SignOut();
        }
    }
}

using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using RockPaperOrleans.Abstractions;

namespace RockPaperOrleans.Grains
{
    public class PlayerGrain : Grain, IPlayerGrain
    {
        public PlayerGrain(
            [PersistentState(nameof(PlayerGrain))] IPersistentState<Player> player,
            ILogger<PlayerGrain> logger)
        {
            Player = player;
            Logger = logger;
        }

        private IPersistentState<Player> Player { get; set; }
        public ILogger<PlayerGrain> Logger { get; set; }
        public IPlayerObserver PlayerObserver { get; set; }

        public override async Task OnActivateAsync()
        {
            Player.State.Name = this.GetGrainIdentity().PrimaryKeyString;

            await base.OnActivateAsync();
        }

        public Task<Player> Get() => Task.FromResult(Player.State);

        public async Task RecordLoss()
        {
            Player.State.LossCount += 1;
            await Player.WriteStateAsync();
        }

        public async Task RecordWin()
        {
            Player.State.WinCount += 1;
            await Player.WriteStateAsync();
        }

        public Task SignIn(IPlayerObserver observer)
        {
            Logger.LogInformation($"Player {Player.State.Name} has signed in in to play.");
            PlayerObserver = observer;
            return Task.CompletedTask;
        }

        public Task SignOut()
        {
            Logger.LogInformation($"Player {Player.State.Name} has signed out.");
            PlayerObserver = null;
            return Task.CompletedTask;
        }

        public Task OpponentSelected(Player opponent)
        {
            Logger.LogInformation($"{opponent.Name} has been chosen to play me, {Player.State.Name}");
            return Task.CompletedTask;
        }
    }
}

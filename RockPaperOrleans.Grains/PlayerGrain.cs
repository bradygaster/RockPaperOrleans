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
            if(PlayerObserver != null)
            {
                Logger.LogInformation($"Recording loss for {Player.State.Name}");
                Player.State.LossCount += 1;
                await PlayerObserver.OnGameLost(Player.State);
                await Player.WriteStateAsync();
            }
        }

        public async Task RecordWin()
        {
            if (PlayerObserver != null)
            {
                Logger.LogInformation($"Recording win for {Player.State.Name}");
                Player.State.WinCount += 1;
                await PlayerObserver.OnGameWon(Player.State);
                await Player.WriteStateAsync();
            }
        }

        public async Task RecordTie()
        {
            if (PlayerObserver != null)
            {
                Logger.LogInformation($"Recording tie for {Player.State.Name}");
                Player.State.TieCount += 1;
                await PlayerObserver.OnGameTied(Player.State);
                await Player.WriteStateAsync();
            }
        }

        public async Task SignIn(IPlayerObserver observer)
        {
            Logger.LogInformation($"Player {Player.State.Name} has signed in in to play.");

            if (observer != null)
            {
                PlayerObserver = observer;
                await PlayerObserver.OnPlayerSignedIn(Player.State);

                // enter the lobby
                var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);
                await lobbyGrain.Enter(Player.State);
            }
        }

        public async Task SignOut()
        {
            Logger.LogInformation($"Player {Player.State.Name} has signed out.");

            PlayerObserver = null;

            // leave the lobby
            var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);
            await lobbyGrain.Leave(Player.State);
        }

        public async Task OpponentSelected(Player opponent)
        {
            Logger.LogInformation($"{Player.State.Name}'s opponent is {opponent.Name}.");
            if (PlayerObserver != null)
            {
                await PlayerObserver.OnOpponentSelected(Player.State, opponent);
            }
        }

        public async Task<Play> Go()
        {
            if (PlayerObserver != null)
            {
                return await PlayerObserver.Go();
            }

            return Play.Unknown;
        }

        public async Task TurnComplete(Turn turn)
        {
            if (PlayerObserver != null)
            {
                Logger.LogInformation($"Recording loss for {Player.State.Name}");
                Player.State.LossCount += 1;
                await PlayerObserver.OnTurnCompleted(turn);
            }
        }
    }
}

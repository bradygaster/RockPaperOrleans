using Orleans;
using Orleans.Runtime;
using RockPaperOrleans.Abstractions;

namespace RockPaperOrleans.Grains
{
    public class LeaderboardGrain : Grain, ILeaderboardGrain
    {
        public HashSet<ILeaderboardGrainObserver> Observers { get; set; } = new();
        public IManagementGrain? ManagementGrain { get; set; }

        public override Task OnActivateAsync()
        {
            ManagementGrain = GrainFactory.GetGrain<IManagementGrain>(0);
            return Task.CompletedTask;
        }

        public Task Subscribe(ILeaderboardGrainObserver observer)
        {
            if (!Observers.Contains(observer))
            {
                Observers.Add(observer);
            }

            return Task.CompletedTask;
        }

        public async Task GameStarted(Game game, Player player1, Player player2)
        {
            foreach (var leaderBoardObserver in Observers)
            {
                await leaderBoardObserver.OnGameStarted(game, player1, player2);
            }
        }

        public async Task TurnStarted(Turn turn, Game game)
        {
            foreach (var leaderBoardObserver in Observers)
            {
                await leaderBoardObserver.OnTurnStarted(turn, game);
            }
        }

        public async Task TurnCompleted(Turn turn, Game game)
        {
            foreach (var leaderBoardObserver in Observers)
            {
                await leaderBoardObserver.OnTurnCompleted(turn, game);
            }
        }

        public async Task TurnScored(Turn turn, Game game)
        {
            foreach (var leaderBoardObserver in Observers)
            {
                await leaderBoardObserver.OnTurnScored(turn, game);
            }
        }

        public async Task GameCompleted(Game game)
        {
            foreach (var leaderBoardObserver in Observers)
            {
                await leaderBoardObserver.OnGameCompleted(game);
            }
        }

        public Task UnSubscribe(ILeaderboardGrainObserver observer)
        {
            if (Observers.Contains(observer))
            {
                Observers.Remove(observer);
            }

            return Task.CompletedTask;
        }

        public async Task LobbyUpdated(List<Player> playersInLobby)
        {
            foreach (var leaderBoardObserver in Observers)
            {
                await leaderBoardObserver.OnLobbyUpdated(playersInLobby);
            }
        }

        public async Task PlayersOnlineUpdated(List<Player> playersOnline)
        {
            foreach (var leaderBoardObserver in Observers)
            {
                playersOnline = playersOnline.OrderByDescending(x => x.PercentWon).ToList();
                await leaderBoardObserver.OnPlayersOnlineUpdated(playersOnline);
            }
        }

        public async Task PlayerScoresUpdated(Player player)
        {
            foreach (var leaderBoardObserver in Observers)
            {
                await leaderBoardObserver.OnPlayerScoresUpdated(player);
            }
        }
    }
}

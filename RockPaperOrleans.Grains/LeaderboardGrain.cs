using Orleans;
using RockPaperOrleans.Abstractions;

namespace RockPaperOrleans.Grains
{
    public class LeaderboardGrain : Grain, ILeaderboardGrain
    {
        public HashSet<ILeaderboardGrainObserver> Observers { get; set; } = new();

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
    }
}

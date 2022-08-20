using Microsoft.AspNetCore.SignalR;
using RockPaperOrleans.Abstractions;

namespace Leaderboard.Hubs
{
    public class LeaderboardObserver : ILeaderboardGrainObserver
    {
        public IHubContext<LeaderboardHub, ILeaderboardGrainObserver> Hub { get; set; }

        public LeaderboardObserver(IHubContext<LeaderboardHub, ILeaderboardGrainObserver> hub)
        {
            Hub = hub;
        }

        public async Task OnGameStarted(Game game, Player player1, Player player2)
        {
            await Hub.Clients.All.OnGameStarted(game, player1, player2);
        }

        public async Task OnTurnStarted(Turn turn, Game game)
        {
            await Hub.Clients.All.OnTurnStarted(turn, game);
        }

        public async Task OnTurnCompleted(Turn turn, Game game)
        {
            await Hub.Clients.All.OnTurnCompleted(turn, game);
        }

        public async Task OnGameCompleted(Game game)
        {
            await Hub.Clients.All.OnGameCompleted(game);
        }
    }

    public class LeaderboardHub : Hub<ILeaderboardGrainObserver>
    {
        public async Task OnGameStarted(Game game, Player player1, Player player2)
        {
            await Clients.All.OnGameStarted(game, player1, player2);
        }

        public async Task OnTurnStarted(Turn turn, Game game)
        {
            await Clients.All.OnTurnStarted(turn, game);
        }

        public async Task OnTurnCompleted(Turn turn, Game game)
        {
            await Clients.All.OnTurnCompleted(turn, game);
        }

        public async Task OnGameCompleted(Game game)
        {
            await Clients.All.OnGameCompleted(game);
        }
    }
}
 
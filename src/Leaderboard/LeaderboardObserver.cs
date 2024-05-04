using Leaderboard.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Leaderboard;

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

    public async Task OnTurnScored(Turn turn, Game game)
    {
        await Hub.Clients.All.OnTurnScored(turn, game);
    }

    public async Task OnGameCompleted(Game game)
    {
        await Hub.Clients.All.OnGameCompleted(game);
    }

    public async Task OnLobbyUpdated(List<Player> playersInLobby)
    {
        await Hub.Clients.All.OnLobbyUpdated(playersInLobby);
    }

    public async Task OnPlayersOnlineUpdated(List<Player> playersOnline)
    {
        await Hub.Clients.All.OnPlayersOnlineUpdated(playersOnline);
    }

    public async Task OnPlayerScoresUpdated(Player player)
    {
        await Hub.Clients.All.OnPlayerScoresUpdated(player);
    }

    public async Task OnSystemStatusUpdated(SystemStatusUpdate update)
    {
        await Hub.Clients.All.OnSystemStatusUpdated(update);
    }
}

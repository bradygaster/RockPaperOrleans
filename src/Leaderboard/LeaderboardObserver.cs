using Leaderboard.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Leaderboard;

public class LeaderboardObserver(IHubContext<LeaderboardHub, ILeaderboardGrainObserver> hub) : ILeaderboardGrainObserver
{
    public async Task OnGameStarted(Game game, Player player1, Player player2)
    {
        await hub.Clients.All.OnGameStarted(game, player1, player2);
    }

    public async Task OnTurnStarted(Turn turn, Game game)
    {
        await hub.Clients.All.OnTurnStarted(turn, game);
    }

    public async Task OnTurnCompleted(Turn turn, Game game)
    {
        await hub.Clients.All.OnTurnCompleted(turn, game);
    }

    public async Task OnTurnScored(Turn turn, Game game)
    {
        await hub.Clients.All.OnTurnScored(turn, game);
    }

    public async Task OnGameCompleted(Game game)
    {
        await hub.Clients.All.OnGameCompleted(game);
    }

    public async Task OnLobbyUpdated(List<Player> playersInLobby)
    {
        await hub.Clients.All.OnLobbyUpdated(playersInLobby);
    }

    public async Task OnPlayersOnlineUpdated(List<Player> playersOnline)
    {
        await hub.Clients.All.OnPlayersOnlineUpdated(playersOnline);
    }

    public async Task OnPlayerScoresUpdated(Player player)
    {
        await hub.Clients.All.OnPlayerScoresUpdated(player);
    }

    public async Task OnSystemStatusUpdated(SystemStatusUpdate update)
    {
        await hub.Clients.All.OnSystemStatusUpdated(update);
    }
}

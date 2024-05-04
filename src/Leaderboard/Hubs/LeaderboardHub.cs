using Microsoft.AspNetCore.SignalR;

namespace Leaderboard.Hubs;

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

    public async Task OnTurnScored(Turn turn, Game game)
    {
        await Clients.All.OnTurnScored(turn, game);
    }

    public async Task OnGameCompleted(Game game)
    {
        await Clients.All.OnGameCompleted(game);
    }

    public async Task OnLobbyUpdated(List<Player> playersInLobby)
    {
        await Clients.All.OnLobbyUpdated(playersInLobby);
    }

    public async Task OnPlayerScoresUpdated(Player player)
    {
        await Clients.All.OnPlayerScoresUpdated(player);
    }

    public async Task OnSystemStatusUpdated(SystemStatusUpdate update)
    {
        await Clients.All.OnSystemStatusUpdated(update);
    }
}

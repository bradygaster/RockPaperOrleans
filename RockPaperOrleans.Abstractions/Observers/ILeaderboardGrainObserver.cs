using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface ILeaderboardGrainObserver : IGrainObserver
    {
        Task OnGameStarted(Game game, Player player1, Player player2);
        Task OnTurnStarted(Turn turn, Game game);
        Task OnTurnCompleted(Turn turn, Game game);
        Task OnTurnScored(Turn turn, Game game);
        Task OnGameCompleted(Game game);
        Task OnLobbyUpdated(List<Player> playersInLobby);
        Task OnPlayersOnlineUpdated(List<Player> playersOnline);
        Task OnPlayerScoresUpdated(Player player);
    }
}

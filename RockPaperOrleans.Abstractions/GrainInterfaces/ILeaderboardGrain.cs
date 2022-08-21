using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface ILeaderboardGrain : IGrainWithGuidKey
    {
        Task GameStarted(Game game, Player player1, Player player2);
        Task TurnStarted(Turn turn, Game game);
        Task TurnCompleted(Turn turn, Game game);
        Task TurnScored(Turn turn, Game game);
        Task GameCompleted(Game game);
        Task Subscribe(ILeaderboardGrainObserver observer);
        Task UnSubscribe(ILeaderboardGrainObserver observer);
        Task LobbyUpdated(List<Player> playersInLobby);
    }
}

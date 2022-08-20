using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface ILeaderboardGrainObserver : IGrainObserver
    {
        Task OnGameStarted(Game game);
        Task OnTurnStarted(Turn turn, Game game);
        Task OnTurnCompleted(Turn turn, Game game);
        Task OnGameCompleted(Game game);
    }
}

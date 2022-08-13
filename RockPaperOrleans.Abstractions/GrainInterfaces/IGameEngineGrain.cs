using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface IGameEngineGrain : IGrainWithGuidKey
    {
        Task<IGameGrain> StartNewGame();
        Task<Game> CurrentGame();
        Task<bool> IsGameComplete();
    }
}

using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface IGameGrain : IGrainWithGuidKey
    {
        Task SelectPlayers();
        Task<Game> GetGame();
        Task SetGame(Game game);
    }
}

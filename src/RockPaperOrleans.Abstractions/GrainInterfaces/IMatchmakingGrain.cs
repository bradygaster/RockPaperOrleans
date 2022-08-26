using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface IMatchmakingGrain : IGrainWithGuidKey
    {
        Task<Tuple<Player, Player>?> ChoosePlayers();
    }
}

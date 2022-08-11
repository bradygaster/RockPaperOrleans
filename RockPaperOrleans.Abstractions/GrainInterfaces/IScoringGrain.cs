using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface IScoringGrain : IGrainWithGuidKey
    {
        Task AlertWinner(Player player);
        Task AlertLoser(Player player);
    }
}

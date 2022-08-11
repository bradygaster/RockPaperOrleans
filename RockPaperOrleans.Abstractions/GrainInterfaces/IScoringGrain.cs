using Orleans;

namespace RockPaperOrleans.Abstractions.GrainInterfaces
{
    public interface IScoringGrain : IGrainWithGuidKey
    {
        Task AlertWinner(Player player);
        Task AlertLoser(Player player);
    }
}

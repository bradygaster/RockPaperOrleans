using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface ILobbyGrain : IGrainWithGuidKey
    {
        Task Enter(Player player);
    }
}

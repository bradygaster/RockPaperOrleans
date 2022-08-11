using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface ILobbyGrain : IGrainWithGuidKey
    {
        Task Enter(Player player);
        Task Leave(Player player);
        Task<List<Player>> GetPlayersInLobby();
    }
}

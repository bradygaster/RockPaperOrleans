using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface IMatchmakingGrain : IGrainWithGuidKey
    {
        Task<Player[]> GetPlayersInLobby(ILobbyGrain lobbyGrain);
        Task<Player[]> ChoosePlayers(Player[] playersInQueue);
    }
}

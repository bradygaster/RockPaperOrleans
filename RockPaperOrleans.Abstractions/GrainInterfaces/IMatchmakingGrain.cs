using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface IMatchmakingGrain : IGrainWithGuidKey
    {
        Task<Player[]> GetPlayersInLobby();
        Task<Tuple<Player, Player>> ChoosePlayers(Player[] playersInQueue);
    }
}

using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface IGameGrain : IGrainWithGuidKey
    {
        Task SelectPlayers(IMatchmakingGrain matchmakingGrain);
        Task NotifyPlayers(Tuple<IPlayer, IPlayer> players);
        Task SubmitPlay(Throw playerThrew);
        Task Score();
    }
}

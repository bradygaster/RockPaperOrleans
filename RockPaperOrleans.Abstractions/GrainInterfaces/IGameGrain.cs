using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface IGameGrain : IGrainWithGuidKey
    {
        Task SelectPlayers(IMatchmakingGrain matchmakingGrain);
        Task NotifyPlayers(Tuple<IStrategy, IStrategy> players);
        Task SubmitPlay(Throw playerThrew);
        Task Score();
    }
}

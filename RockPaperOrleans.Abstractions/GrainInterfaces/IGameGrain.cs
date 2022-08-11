using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface IGameGrain : IGrainWithGuidKey
    {
        Task SelectPlayers();
        Task SubmitPlay(Throw playerThrew);
        Task Score();
    }
}

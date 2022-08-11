using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface IPlayerGrain : IGrainWithStringKey
    {
        Task RecordWin();
        Task RecordLoss();
        Task<Player> Get();
    }
}

using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface IPlayerGrain : IGrainWithStringKey
    {
        Task Play(Play play);
        Task RecordWin();
        Task RecordLoss();
        Task<Player> Get();
    }
}

using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface IPlayerGrain : IGrainWithStringKey
    {
        Task RecordWin();
        Task RecordLoss();
        Task RecordTie();
        Task<Player> Get();
        Task SignIn(IPlayerObserver observer);
        Task SignOut();
        Task OpponentSelected(Player opponent);
        Task<Play> Go();
    }
}

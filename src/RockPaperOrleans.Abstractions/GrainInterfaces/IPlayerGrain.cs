using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface IPlayerGrain : IGrainWithStringKey
    {
        Task RecordWin(Player opponent);
        Task RecordLoss(Player opponent);
        Task RecordTie(Player opponent);
        Task<Player> Get();
        Task SignIn(IPlayerObserver observer);
        Task SignOut();
        Task OpponentSelected(Player opponent);
        Task<Play> Go();
        Task TurnComplete(Turn turn);
        Task<bool> IsPlayerOnline();
    }
}

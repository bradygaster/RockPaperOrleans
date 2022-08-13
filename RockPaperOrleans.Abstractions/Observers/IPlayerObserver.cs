using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface IPlayerObserver : IGrainObserver
    {
        Task OnPlayerSignedIn(Player player);
        Task OnPlayerSignedOut(Player player);
        Task OnOpponentSelected(Player opponent);
    }
}

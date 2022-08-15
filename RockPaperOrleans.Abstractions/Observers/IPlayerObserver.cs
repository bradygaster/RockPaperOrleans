using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface IPlayerObserver : IGrainObserver
    {
        Task<Play> Go();
        Task OnPlayerSignedIn(Player player);
        Task OnPlayerSignedOut(Player player);
        Task OnOpponentSelected(Player player, Player opponent);
        Task OnGameWon(Player player);
        Task OnGameLost(Player player);
        Task OnGameTied(Player player);
    }
}

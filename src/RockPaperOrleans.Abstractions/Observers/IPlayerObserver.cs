namespace RockPaperOrleans.Abstractions;

public interface IPlayerObserver : IGrainObserver
{
    Task<Play> Go();
    Task OnPlayerSignedIn(Player player);
    Task OnPlayerSignedOut(Player player);
    Task OnOpponentSelected(Player player, Player opponent);
    Task OnGameWon(Player player, Player opponent);
    Task OnGameLost(Player player, Player opponent);
    Task OnGameTied(Player player, Player opponent);
    Task OnTurnCompleted(Turn turn);
}

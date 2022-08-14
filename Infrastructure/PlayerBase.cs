using Microsoft.Extensions.Logging;
using RockPaperOrleans.Abstractions;

namespace RockPaperOrleans
{
    public abstract class PlayerBase : IPlayerObserver
    {
        public Player Opponent { get; private set; }

        public abstract Task<Play> Go();

        public virtual Task OnOpponentSelected(Player opponent)
        {
            Opponent = opponent;
            return Task.CompletedTask;
        }

        public virtual Task OnPlayerSignedIn(Player player)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnPlayerSignedOut(Player player)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnGameWon(Player player)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnGameLost(Player player)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnGameTied(Player player)
        {
            return Task.CompletedTask;
        }
    }
}

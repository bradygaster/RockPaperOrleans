using RockPaperOrleans.Abstractions;

namespace RockPaperOrleans
{
    public abstract class PlayerBase : IPlayerObserver
    {
        public abstract Task<Play> Go();

        public virtual Task OnOpponentSelected(Player opponent)
        {
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
            return Task.FromResult(Play.Paper);
        }

        public virtual Task OnGameLost(Player player)
        {
            return Task.FromResult(Play.Paper);
        }
    }
}

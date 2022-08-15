using Microsoft.Extensions.Logging;
using RockPaperOrleans.Abstractions;

namespace RockPaperOrleans
{
    public abstract class PlayerBase : IPlayerObserver
    {
        public Player Opponent { get; private set; }
        public ILogger Logger { get; set; }

        protected PlayerBase(ILogger logger)
        {
            Logger = logger;
        }

        public abstract Task<Play> Go();

        public virtual Task OnPlayerSignedIn(Player player)
        {
            Logger.LogInformation($"{GetType().Name} signed in.");
            return Task.CompletedTask;
        }

        public virtual Task OnPlayerSignedOut(Player player)
        {
            Logger.LogInformation($"{GetType().Name} signed out.");
            return Task.CompletedTask;
        }

        public virtual Task OnOpponentSelected(Player opponent)
        {
            Opponent = opponent;
            Logger.LogInformation($"{GetType().Name} is about to play {opponent.Name}.");
            return Task.CompletedTask;
        }

        public virtual Task OnGameWon(Player player)
        {
            Logger.LogInformation($"{GetType().Name} wins against {Opponent.Name}.");
            return Task.CompletedTask;
        }

        public virtual Task OnGameLost(Player player)
        {
            Logger.LogInformation($"{GetType().Name} loses to {Opponent.Name}.");
            return Task.CompletedTask;
        }

        public virtual Task OnGameTied(Player player)
        {
            Logger.LogInformation($"{GetType().Name} ties with {Opponent.Name}.");
            return Task.CompletedTask;
        }
    }
}

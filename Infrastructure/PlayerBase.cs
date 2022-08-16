using Microsoft.Extensions.Logging;
using RockPaperOrleans.Abstractions;
using System.Numerics;

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
            Logger.LogInformation($"{player.Name} signed in.");
            return Task.CompletedTask;
        }

        public virtual Task OnPlayerSignedOut(Player player)
        {
            Logger.LogInformation($"{player.Name} signed out.");
            return Task.CompletedTask;
        }

        public virtual Task OnOpponentSelected(Player player, Player opponent)
        {
            Opponent = opponent;
            Logger.LogInformation($"{player.Name} is about to play {opponent.Name}.");
            return Task.CompletedTask;
        }

        public Task OnTurnCompleted(Turn turn)
        {
            if(string.IsNullOrEmpty(turn.Winner) || turn.Winner.ToLower() == "tie")
            {
                Logger.LogInformation($"{turn.Throws[0].Player} ties this round with {turn.Throws[1].Player}, throwing {turn.Throws[0].Play}.");
            }
            else
            {
                Logger.LogInformation($"{turn.Winner} wins round against {turn.Throws.First(x => x.Player != turn.Winner).Player}, throwing {turn.Throws.First(x => x.Player == turn.Winner).Play}.");
            }

            return Task.CompletedTask;
        }

        public virtual Task OnGameWon(Player player)
        {
            Logger.LogInformation($"{player.Name} wins against {Opponent.Name}.");
            return Task.CompletedTask;
        }

        public virtual Task OnGameLost(Player player)
        {
            Logger.LogInformation($"{player.Name} loses to {Opponent.Name}.");
            return Task.CompletedTask;
        }

        public virtual Task OnGameTied(Player player)
        {
            Logger.LogInformation($"{player.Name} ties with {Opponent.Name}.");
            return Task.CompletedTask;
        }
    }
}

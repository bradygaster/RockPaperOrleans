using RockPaperOrleans.Abstractions;

namespace Scissors
{
    internal class ScissorsPlayer : IPlayerObserver
    {
        public ILogger<ScissorsPlayer> Logger { get; set; }

        public ScissorsPlayer(ILogger<ScissorsPlayer> logger)
        {
            Logger = logger;
        }

        public Task OnOpponentSelected(Player opponent)
        {
            Logger.LogInformation($"{nameof(ScissorsPlayer)} playing against {opponent.Name}");
            return Task.CompletedTask;
        }

        public Task OnPlayerSignedIn(Player player)
        {
            Logger.LogInformation($"{player.Name} signed in.");
            return Task.CompletedTask;
        }

        public Task OnPlayerSignedOut(Player player)
        {
            Logger.LogInformation($"{player.Name} signed out.");
            return Task.CompletedTask;
        }

        public Task<Play> Go()
        {
            Logger.LogInformation("I cut things!");
            return Task.FromResult(Play.Scissors);
        }

        public Task OnGameWon(Player player)
        {
            Logger.LogInformation("I cut it up!");
            return Task.FromResult(Play.Paper);
        }

        public Task OnGameLost(Player player)
        {
            Logger.LogInformation("I got crushed.");
            return Task.FromResult(Play.Paper);
        }
    }
}

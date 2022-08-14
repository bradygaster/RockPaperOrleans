using RockPaperOrleans.Abstractions;

namespace Paper
{
    internal class PaperPlayer : IPlayerObserver
    {
        public ILogger<PaperPlayer> Logger { get; set; }

        public PaperPlayer(ILogger<PaperPlayer> logger)
        {
            Logger = logger;
        }

        public Task OnOpponentSelected(Player opponent)
        {
            Logger.LogInformation($"{nameof(PaperPlayer)} playing against {opponent.Name}");
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
            Logger.LogInformation("Playing paper, like always!");
            return Task.FromResult(Play.Paper);
        }

        public Task OnGameWon(Player player)
        {
            Logger.LogInformation("Yes, I won!");
            return Task.FromResult(Play.Paper);
        }

        public Task OnGameLost(Player player)
        {
            Logger.LogInformation("Shucks");
            return Task.FromResult(Play.Paper);
        }
    }
}

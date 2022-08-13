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
            Logger.LogInformation($"Paper playing against {opponent.Name}");
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
    }
}

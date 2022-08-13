using RockPaperOrleans.Abstractions;

namespace Rock
{
    internal class RockPlayer : IPlayerObserver
    {
        public ILogger<RockPlayer> Logger { get; set; }

        public RockPlayer(ILogger<RockPlayer> logger)
        {
            Logger = logger;
        }

        public Task OnOpponentSelected(Player opponent)
        {
            Logger.LogInformation($"{nameof(RockPlayer)} playing against {opponent.Name}");
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
            Logger.LogInformation("I like rocks!");
            return Task.FromResult(Play.Rock);
        }
    }
}

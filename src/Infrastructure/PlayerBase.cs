namespace RockPaperOrleans;

public abstract class PlayerBase : IPlayerObserver
{
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

    public virtual Task OnGameWon(Player player, Player opponent)
    {
        Logger.LogInformation($"{player.Name} wins against {opponent.Name}.");
        return Task.CompletedTask;
    }

    public virtual Task OnGameLost(Player player, Player opponent)
    {
        Logger.LogInformation($"{player.Name} loses to {opponent.Name}.");
        return Task.CompletedTask;
    }

    public virtual Task OnGameTied(Player player, Player opponent)
    {
        Logger.LogInformation($"{player.Name} ties with {opponent.Name}.");
        return Task.CompletedTask;
    }
}

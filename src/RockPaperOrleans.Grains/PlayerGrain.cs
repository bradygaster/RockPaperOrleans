namespace RockPaperOrleans.Grains;

public class PlayerGrain : Grain, IPlayerGrain
{
    public PlayerGrain([PersistentState("Players", storageName: "Players")] IPersistentState<Player> player,
        ILogger<PlayerGrain> logger)
    {
        Player = player;
        Logger = logger;
    }

    private IPersistentState<Player> Player { get; set; }
    public ILogger<PlayerGrain> Logger { get; set; }
    public IPlayerObserver? PlayerObserver { get; set; }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Player.State.Name = this.GetPrimaryKeyString();

        await base.OnActivateAsync(cancellationToken);
    }

    public async Task<Play> Go()
    {
        if (PlayerObserver != null)
        {
            return await PlayerObserver.Go();
        }

        return Play.Unknown;
    }

    public Task<Player> Get() 
        => Task.FromResult(Player.State);

    public async Task SignIn(IPlayerObserver observer)
    {
        Logger.LogInformation($"RPO: Player {Player.State.Name} has signed in in to play.");

        if (observer != null)
        {
            PlayerObserver = observer;
            Player.State.IsActive = true;

            var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);
            await lobbyGrain.SignIn(Player.State);
            await lobbyGrain.EnterLobby(Player.State);
            await Player.WriteStateAsync();
        }
    }

    public async Task SignOut()
    {
        Player.State.IsActive = false;

        Logger.LogInformation($"RPO: Player {Player.State.Name} has signed out.");

        PlayerObserver = null;

        var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);
        await lobbyGrain.SignOut(Player.State);
        await Player.WriteStateAsync();
    }

    public Task<bool> IsPlayerOnline()
        => Task.FromResult<bool>(Player.State.IsActive);

    public async Task OpponentSelected(Player opponent)
    {
        Logger.LogInformation($"RPO: {Player.State.Name}'s opponent is {opponent.Name}.");
    }

    public async Task TurnComplete(Turn turn)
    {
        if (PlayerObserver != null)
        {
            Logger.LogInformation($"RPO: Turn complete.");
        }
    }

    public async Task RecordLoss(Player opponent)
    {
        if(PlayerObserver != null)
        {
            Logger.LogInformation($"RPO: Recording loss for {Player.State.Name}");
            Player.State.TotalGamesPlayed += 1;
            Player.State.LossCount += 1;
            Player.State.PercentWon = (int)Player.State.CalculateWinPercentage();
            await Player.WriteStateAsync();
            await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).PlayerScoresUpdated(Player.State);
        }
    }

    public async Task RecordWin(Player opponent)
    {
        if (PlayerObserver != null)
        {
            Logger.LogInformation($"RPO: Recording win for {Player.State.Name}");
            Player.State.TotalGamesPlayed += 1;
            Player.State.WinCount += 1;
            Player.State.PercentWon = (int)Player.State.CalculateWinPercentage();
            await Player.WriteStateAsync();
            await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).PlayerScoresUpdated(Player.State);
        }
    }

    public async Task RecordTie(Player opponent)
    {
        if (PlayerObserver != null)
        {
            Logger.LogInformation($"RPO: Recording tie for {Player.State.Name}");
            Player.State.TotalGamesPlayed += 1;
            Player.State.TieCount += 1;
            Player.State.PercentWon = (int)Player.State.CalculateWinPercentage();
            await Player.WriteStateAsync();
            await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).PlayerScoresUpdated(Player.State);
        }
    }
}

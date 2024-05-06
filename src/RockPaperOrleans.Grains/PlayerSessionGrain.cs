namespace RockPaperOrleans.Grains;

public class PlayerSessionGrain : Grain, IPlayerSessionGrain
{
    public PlayerSessionGrain([PersistentState("Players", storageName: "Players")] IPersistentState<Player> player,
        ILogger<PlayerSessionGrain> logger)
    {
        Player = player;
        Logger = logger;
    }

    private IPersistentState<Player> Player { get; set; }
    public ILogger<PlayerSessionGrain> Logger { get; set; }
    public Player? Opponent { get; set; }
    public IPlayerGrain? PlayerGrain { get; set; }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Player.State.Name = this.GetPrimaryKeyString();

        await base.OnActivateAsync(cancellationToken);
    }

    public async Task<Play> Go()
    {
        if (PlayerGrain is not null && Opponent is not null)
        {
            return await PlayerGrain.Go(Opponent);
        }

        return Play.Unknown;
    }

    public Task<Player> Get()
        => Task.FromResult(Player.State);

    public async Task SignIn(IPlayerGrain playerGrain)
    {
        Logger.LogInformation("RPO: Player {PlayerName} has signed in in to play.", Player.State.Name);

        PlayerGrain = playerGrain;
        Player.State.IsActive = true;

        var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);
        await lobbyGrain.SignIn(Player.State);
        await lobbyGrain.EnterLobby(Player.State);
        await Player.WriteStateAsync();
    }

    public async Task SignOut()
    {
        Player.State.IsActive = false;

        Logger.LogInformation("RPO: Player {PlayerName} has signed out.", Player.State.Name);

        PlayerGrain = null;

        var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);
        await lobbyGrain.SignOut(Player.State);
        await Player.WriteStateAsync();
    }

    public Task<bool> IsPlayerOnline()
        => Task.FromResult<bool>(Player.State.IsActive);

    public Task OpponentSelected(Player opponent)
    {
        Opponent = opponent;
        Logger.LogInformation("RPO: {PlayerName}'s opponent is {OpponentName}.", Player.State.Name, opponent.Name);
        return Task.CompletedTask;
    }

    public Task TurnComplete(Turn turn)
    {
        Logger.LogInformation($"RPO: Turn complete.");
        return Task.CompletedTask;
    }

    public async Task RecordLoss(Player opponent)
    {
        Logger.LogInformation("RPO: Recording loss for {PlayerName}", Player.State.Name);
        Player.State.TotalGamesPlayed += 1;
        Player.State.LossCount += 1;
        Player.State.PercentWon = (int)Player.State.CalculateWinPercentage();
        await Player.WriteStateAsync();
        await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).PlayerScoresUpdated(Player.State);
    }

    public async Task RecordWin(Player opponent)
    {
        Logger.LogInformation("RPO: Recording win for {PlayerName}", Player.State.Name);
        Player.State.TotalGamesPlayed += 1;
        Player.State.WinCount += 1;
        Player.State.PercentWon = (int)Player.State.CalculateWinPercentage();
        await Player.WriteStateAsync();
        await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).PlayerScoresUpdated(Player.State);
    }

    public async Task RecordTie(Player opponent)
    {
        Logger.LogInformation("RPO: Recording tie for {PlayerName}", Player.State.Name);
        Player.State.TotalGamesPlayed += 1;
        Player.State.TieCount += 1;
        Player.State.PercentWon = (int)Player.State.CalculateWinPercentage();
        await Player.WriteStateAsync();
        await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).PlayerScoresUpdated(Player.State);
    }
}

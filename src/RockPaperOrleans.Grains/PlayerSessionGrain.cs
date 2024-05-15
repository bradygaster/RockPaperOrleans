namespace RockPaperOrleans.Grains;

public class PlayerSessionGrain(
    [PersistentState("Players", storageName: "Players")] IPersistentState<Player> player,
    ILogger<PlayerSessionGrain> logger) : Grain, IPlayerSessionGrain
{
    private Player? _opponent;
    private IPlayerGrain? _playerGrain;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        player.State.Name = this.GetPrimaryKeyString();

        await base.OnActivateAsync(cancellationToken);
    }

    public async Task<Play> Go()
    {
        if (_playerGrain is not null && _opponent is not null)
        {
            return await _playerGrain.Go(_opponent);
        }

        return Play.Unknown;
    }

    public Task<Player> Get() => Task.FromResult(player.State);

    public async Task SignIn(IPlayerGrain playerGrain)
    {
        if (!playerGrain.Equals(_playerGrain))
        {
            logger.LogInformation("RPO: Player {PlayerName} has signed in in to play.", player.State.Name);
            _playerGrain = playerGrain;
            player.State.IsActive = true;
            await player.WriteStateAsync();
        }

        var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);
        await lobbyGrain.SignIn(player.State);
        await lobbyGrain.EnterLobby(player.State);
    }

    public async Task SignOut()
    {
        player.State.IsActive = false;

        logger.LogInformation("RPO: Player {PlayerName} has signed out.", player.State.Name);

        _playerGrain = null;

        var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);
        await lobbyGrain.SignOut(player.State);
        await player.WriteStateAsync();
    }

    public Task<bool> IsPlayerOnline()
        => Task.FromResult(player.State.IsActive);

    public Task OpponentSelected(Player opponent)
    {
        _opponent = opponent;
        logger.LogInformation("RPO: {PlayerName}'s opponent is {OpponentName}.", player.State.Name, opponent.Name);
        return Task.CompletedTask;
    }

    public async Task TurnComplete(Turn turn)
    {
        logger.LogInformation($"RPO: Turn complete.");
        
        if(_playerGrain is not null)
        {
            await _playerGrain.OnTurnCompleted(turn);
        }
    }

    public async Task RecordLoss(Player opponent)
    {
        logger.LogInformation("RPO: Recording loss for {PlayerName}", player.State.Name);
        player.State.TotalGamesPlayed += 1;
        player.State.LossCount += 1;
        player.State.PercentWon = (int)player.State.CalculateWinPercentage();
        await player.WriteStateAsync();
        await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).PlayerScoresUpdated(player.State);
    }

    public async Task RecordWin(Player opponent)
    {
        logger.LogInformation("RPO: Recording win for {PlayerName}", player.State.Name);
        player.State.TotalGamesPlayed += 1;
        player.State.WinCount += 1;
        player.State.PercentWon = (int)player.State.CalculateWinPercentage();
        await player.WriteStateAsync();
        await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).PlayerScoresUpdated(player.State);
    }

    public async Task RecordTie(Player opponent)
    {
        logger.LogInformation("RPO: Recording tie for {PlayerName}", player.State.Name);
        player.State.TotalGamesPlayed += 1;
        player.State.TieCount += 1;
        player.State.PercentWon = (int)player.State.CalculateWinPercentage();
        await player.WriteStateAsync();
        await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).PlayerScoresUpdated(player.State);
    }
}

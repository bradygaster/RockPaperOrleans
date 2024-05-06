namespace RockPaperOrleans.Grains;

public class LobbyGrain : Grain, ILobbyGrain
{
    private readonly IPersistentState<List<Player>> _playersInLobby;
    private readonly IPersistentState<List<Player>> _playersSignedIn;
    private readonly ILogger<LobbyGrain> _logger;
    private readonly ILeaderboardGrain _leaderboardGrain;

    public LobbyGrain(
        [PersistentState("InLobby", storageName: "Lobby")] IPersistentState<List<Player>> playersInLobby, 
        [PersistentState("SignedIn", storageName: "Lobby")] IPersistentState<List<Player>> playersSignedIn,
        ILogger<LobbyGrain> logger)
    {
        _playersInLobby = playersInLobby;
        _playersSignedIn = playersSignedIn;
        _logger = logger;
        _leaderboardGrain = GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
    }

    public Task<List<Player>> GetPlayersInLobby() 
        => Task.FromResult(_playersInLobby.State);

    public async Task SignIn(Player player)
    {
        if (!_playersSignedIn.State.Any(x => x.Name == player.Name))
        {
            _logger.LogInformation("RPO: {PlayerName} has entered the game.", player.Name);
            _playersSignedIn.State.Add(player);
            await _leaderboardGrain.PlayersOnlineUpdated(_playersSignedIn.State);
        }
    }

    public async Task EnterLobby(Player player)
    {
        if (!_playersInLobby.State.Any(x => x.Name == player.Name))
        {
            _logger.LogInformation("RPO: {PlayerName} has entered the lobby.", player.Name);
            _playersInLobby.State.Add(player);
            await _leaderboardGrain.LobbyUpdated(_playersInLobby.State);

            UpdatePlayerLeaderboardRecord(player);
            await _leaderboardGrain.PlayersOnlineUpdated(_playersSignedIn.State);
        }
    }

    public async Task EnterGame(Player player)
    {
        if (_playersInLobby.State.Any(x => x.Name == player.Name))
        {
            _logger.LogInformation("RPO: {PlayerName} has left the lobby and entered the game.", player.Name);
            _playersInLobby.State.RemoveAll(x => x.Name == player.Name);
            await _leaderboardGrain.LobbyUpdated(_playersInLobby.State);
            await _leaderboardGrain.PlayersOnlineUpdated(_playersSignedIn.State);
        }
    }

    public async Task SignOut(Player player)
    {
        if (_playersInLobby.State.Any(x => x.Name == player.Name))
        {
            _logger.LogInformation("RPO: {PlayerName} has left the lobby.", player.Name);
            _playersInLobby.State.RemoveAll(x => x.Name == player.Name);
            await _leaderboardGrain.LobbyUpdated(_playersInLobby.State);
        }

        if (_playersSignedIn.State.Any(x => x.Name == player.Name))
        {
            _logger.LogInformation("RPO: {PlayerName} has left the game.", player.Name);
            _playersSignedIn.State.RemoveAll(x => x.Name == player.Name);
        }

        await _leaderboardGrain.PlayersOnlineUpdated(_playersSignedIn.State);
    }

    private void UpdatePlayerLeaderboardRecord(Player player)
    {
        if (_playersSignedIn.State.Any(x => x.Name == player.Name))
        {
            _playersSignedIn.State.RemoveAll(x => x.Name == player.Name);
            _playersSignedIn.State.Add(player);
        }
    }
}

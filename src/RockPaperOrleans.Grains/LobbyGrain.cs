namespace RockPaperOrleans.Grains;

public class LobbyGrain : Grain, ILobbyGrain
{
    private IPersistentState<List<Player>> PlayersInLobby { get; set; }
    private IPersistentState<List<Player>> PlayersSignedIn { get; set; }
    public ILogger<LobbyGrain> Logger { get; set; }

    public LobbyGrain(
        [PersistentState("Lobby", storageName: "Lobby")] IPersistentState<List<Player>> playersInLobby, 
        [PersistentState("Lobby", storageName: "Lobby")] IPersistentState<List<Player>> playersSignedIn,
        ILogger<LobbyGrain> logger)
    {
        PlayersInLobby = playersInLobby;
        PlayersSignedIn = playersSignedIn;
        Logger = logger;
    }

    public Task<List<Player>> GetPlayersInLobby() 
        => Task.FromResult(PlayersInLobby.State);

    public async Task SignIn(Player player)
    {
        if (!PlayersSignedIn.State.Any(x => x.Name == player.Name))
        {
            Logger.LogInformation("RPO: {PlayerName} has entered the game.", player.Name);
            PlayersSignedIn.State.Add(player);
            await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).PlayersOnlineUpdated(PlayersSignedIn.State);
        }
    }

    public async Task EnterLobby(Player player)
    {
        if (!PlayersInLobby.State.Any(x => x.Name == player.Name))
        {
            Logger.LogInformation("RPO: {PlayerName} has entered the lobby.", player.Name);
            PlayersInLobby.State.Add(player);
            await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).LobbyUpdated(PlayersInLobby.State);

            UpdatePlayerLeaderboardRecord(player);
            await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).PlayersOnlineUpdated(PlayersSignedIn.State);
        }
    }

    public async Task EnterGame(Player player)
    {
        if (PlayersInLobby.State.Any(x => x.Name == player.Name))
        {
            Logger.LogInformation("RPO: {PlayerName} has left the lobby and entered the game.", player.Name);
            PlayersInLobby.State.RemoveAll(x => x.Name == player.Name);
            await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).LobbyUpdated(PlayersInLobby.State);
            await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).PlayersOnlineUpdated(PlayersSignedIn.State);
        }
    }

    public async Task SignOut(Player player)
    {
        if (PlayersInLobby.State.Any(x => x.Name == player.Name))
        {
            Logger.LogInformation("RPO: {PlayerName} has left the lobby.", player.Name);
            PlayersInLobby.State.RemoveAll(x => x.Name == player.Name);
            await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).LobbyUpdated(PlayersInLobby.State);
        }

        if (PlayersSignedIn.State.Any(x => x.Name == player.Name))
        {
            Logger.LogInformation("RPO: {PlayerName} has left the game.", player.Name);
            PlayersSignedIn.State.RemoveAll(x => x.Name == player.Name);
        }

        await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).PlayersOnlineUpdated(PlayersSignedIn.State);
    }

    private void UpdatePlayerLeaderboardRecord(Player player)
    {
        if (PlayersSignedIn.State.Any(x => x.Name == player.Name))
        {
            PlayersSignedIn.State.RemoveAll(x => x.Name == player.Name);
            PlayersSignedIn.State.Add(player);
        }
    }
}

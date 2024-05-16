namespace RockPaperOrleans.Grains;

public class MatchmakingGrain(ILogger<MatchmakingGrain> logger) : Grain, IMatchmakingGrain
{
    public async Task<(Player, Player)?> ChoosePlayers()
    {
        var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);
        var lobby = await lobbyGrain.GetPlayersInLobby();

        logger.LogInformation("RPO: There are {Count} players in the lobby.", lobby.Count);

        if (lobby is not { Count: >= 2 })
        {
            return default;
        }

        var pickPlayers = new Func<Player[]>(() => Random.Shared.GetItems(lobby.ToArray(), 2));

        var players = pickPlayers();
        while (players[0].Name == players[1].Name) players = pickPlayers();

        await lobbyGrain.EnterGame(players[0]);
        await lobbyGrain.EnterGame(players[1]);

        return (players[0], players[1]);
    }
}

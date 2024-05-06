namespace RockPaperOrleans.Grains;

public class LeaderboardGrain : Grain, ILeaderboardGrain
{
    private readonly HashSet<ILeaderboardGrainObserver> _observers = [];

    public Task Subscribe(ILeaderboardGrainObserver observer)
    {
        _observers.Add(observer);
        return Task.CompletedTask;
    }

    public async Task GameStarted(Game game, Player player1, Player player2)
    {
        foreach (var leaderBoardObserver in _observers)
        {
            await leaderBoardObserver.OnGameStarted(game, player1, player2);
        }
    }

    public async Task TurnStarted(Turn turn, Game game)
    {
        foreach (var leaderBoardObserver in _observers)
        {
            await leaderBoardObserver.OnTurnStarted(turn, game);
        }
    }

    public async Task TurnCompleted(Turn turn, Game game)
    {
        foreach (var leaderBoardObserver in _observers)
        {
            await leaderBoardObserver.OnTurnCompleted(turn, game);
        }
    }

    public async Task TurnScored(Turn turn, Game game)
    {
        foreach (var leaderBoardObserver in _observers)
        {
            await leaderBoardObserver.OnTurnScored(turn, game);
        }
    }

    public async Task GameCompleted(Game game)
    {
        foreach (var leaderBoardObserver in _observers)
        {
            await leaderBoardObserver.OnGameCompleted(game);
        }
    }

    public Task UnSubscribe(ILeaderboardGrainObserver observer)
    {
        _observers.Remove(observer);
        return Task.CompletedTask;
    }

    public async Task LobbyUpdated(List<Player> playersInLobby)
    {
        foreach (var leaderBoardObserver in _observers)
        {
            await leaderBoardObserver.OnLobbyUpdated(playersInLobby);
        }
    }

    public async Task PlayersOnlineUpdated(List<Player> playersOnline)
    {
        playersOnline = [.. playersOnline.OrderByDescending(x => x.PercentWon)];
        foreach (var leaderBoardObserver in _observers)
        {
            await leaderBoardObserver.OnPlayersOnlineUpdated(playersOnline);
        }
    }

    public async Task PlayerScoresUpdated(Player player)
    {
        foreach (var leaderBoardObserver in _observers)
        {
            await leaderBoardObserver.OnPlayerScoresUpdated(player);
        }
    }

    public async Task UpdateSystemStatus(SystemStatusUpdate update)
    {
        foreach (var leaderBoardObserver in _observers)
        {
            await leaderBoardObserver.OnSystemStatusUpdated(update);
        }
    }
}

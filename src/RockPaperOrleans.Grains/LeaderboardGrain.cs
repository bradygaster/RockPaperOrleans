using Orleans.Utilities;

namespace RockPaperOrleans.Grains;

public class LeaderboardGrain(ILogger<LeaderboardGrain> logger) : Grain, ILeaderboardGrain
{
    private readonly ObserverManager<ILeaderboardGrainObserver> _observers = new(TimeSpan.FromMinutes(1), logger);

    public Task Subscribe(ILeaderboardGrainObserver observer)
    {
        _observers.Subscribe(observer, observer);
        return Task.CompletedTask;
    }

    public async Task GameStarted(Game game, Player player1, Player player2)
    {
        await _observers.Notify(observer => observer.OnGameStarted(game, player1, player2));
    }

    public async Task TurnStarted(Turn turn, Game game)
    {
        await _observers.Notify(observer => observer.OnTurnStarted(turn, game));
    }

    public async Task TurnCompleted(Turn turn, Game game)
    {
        await _observers.Notify(observer => observer.OnTurnCompleted(turn, game));
    }

    public async Task TurnScored(Turn turn, Game game)
    {
        await _observers.Notify(observer => observer.OnTurnScored(turn, game));
    }

    public async Task GameCompleted(Game game)
    {
        await _observers.Notify(observer => observer.OnGameCompleted(game));
    }

    public Task UnSubscribe(ILeaderboardGrainObserver observer)
    {
        _observers.Unsubscribe(observer);
        return Task.CompletedTask;
    }

    public async Task LobbyUpdated(List<Player> playersInLobby)
    {
        await _observers.Notify(observer => observer.OnLobbyUpdated(playersInLobby));
    }

    public async Task PlayersOnlineUpdated(List<Player> playersOnline)
    {
        List<Player> orderedPlayers = [.. playersOnline.OrderByDescending(x => x.PercentWon)];
        await _observers.Notify(observer => observer.OnPlayersOnlineUpdated(orderedPlayers));
    }

    public async Task PlayerScoresUpdated(Player player)
    {
        await _observers.Notify(observer => observer.OnPlayerScoresUpdated(player));
    }

    public async Task UpdateSystemStatus(SystemStatusUpdate update)
    {
        await _observers.Notify(observer => observer.OnSystemStatusUpdated(update));
    }
}

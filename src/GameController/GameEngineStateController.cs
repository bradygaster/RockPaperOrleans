using System.Diagnostics;

namespace GameController;

public class GameEngineStateController(IGrainFactory grainFactory)
{
    private IGrainFactory _grainFactory = grainFactory;

    public bool IsStarted { get; private set; } = true;
    public int GameLoopDelayInMilliseconds { get; private set; } = 256;
    public int GamesCompleted { get; private set; } = 0;
    public DateTime ServerStartedTime { get; private set; } = DateTime.UtcNow;
    public Stopwatch Uptime { get; private set; } = Stopwatch.StartNew();

    public void Start() => IsStarted = true;
    public void Stop() => IsStarted = false;
    public void SetGameLoopDelayInterval(int gameLoopDelayInMilliseconds) => GameLoopDelayInMilliseconds = gameLoopDelayInMilliseconds;
    public void IncrementGameCount() => GamesCompleted++;
    public async Task KickPlayer(string playerName) => await _grainFactory.GetGrain<IPlayerSessionGrain>(playerName).KickPlayer();
    public async Task UnkickPlayer(string playerName) => await _grainFactory.GetGrain<IPlayerSessionGrain>(playerName).UnkickPlayer();
}

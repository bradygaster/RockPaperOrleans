
namespace RockPaperOrleans.Abstractions;

public abstract class BasePlayerGrain : Grain, IPlayerGrain
{
    public abstract Task<Play> Go(Player opponent);

    public virtual Task OnTurnCompleted(Turn turn) => Task.CompletedTask;
}

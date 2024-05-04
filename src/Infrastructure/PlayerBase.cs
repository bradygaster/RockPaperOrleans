namespace RockPaperOrleans;

public abstract class PlayerBase : IPlayerObserver
{
    public abstract Task<Play> Go();
}

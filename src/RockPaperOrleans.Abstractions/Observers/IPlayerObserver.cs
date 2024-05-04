namespace RockPaperOrleans.Abstractions;

public interface IPlayerObserver : IGrainObserver
{
    Task<Play> Go();
}

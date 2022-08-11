using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface IStrategy : IGrainObserver
    {
        Task<Play> Play();
    }
}

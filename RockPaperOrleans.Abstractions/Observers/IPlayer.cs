using Orleans;

namespace RockPaperOrleans.Abstractions.Observers
{
    public interface IPlayer : IGrainObserver
    {
        Task OnOpponentSelected(Player player);
        Task<Play> Play();
    }
}

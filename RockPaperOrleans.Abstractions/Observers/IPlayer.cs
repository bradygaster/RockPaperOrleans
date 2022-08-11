using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface IPlayer : IGrainObserver
    {
        Task<Player> Get();
        Task OnSelected(Game game);
        Task<Play> Play();
    }
}

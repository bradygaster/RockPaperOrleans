namespace RockPaperOrleans.Abstractions.Observers
{
    public interface IPlayer
    {
        Task OnOpponentSelected(Player player);
        Task<Play> Play();
    }
}

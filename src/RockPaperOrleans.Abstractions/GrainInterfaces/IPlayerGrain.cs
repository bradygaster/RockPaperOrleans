namespace RockPaperOrleans.Abstractions;

public interface IPlayerGrain : IGrainWithStringKey
{
    Task<Play> Go(Player opponent);
    Task OnTurnCompleted(Turn turn);
}

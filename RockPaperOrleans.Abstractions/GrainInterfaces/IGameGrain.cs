using Orleans;

namespace RockPaperOrleans.Abstractions
{
    public interface IGameGrain : IGrainWithGuidKey
    {
        Task EnlistPlayers(Player player1, Player player2);
        Task Go(Throw thrown);
        Task Score();
    }
}

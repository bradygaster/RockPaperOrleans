namespace RockPaperOrleans.Abstractions
{
    public interface IGameControllerGrain
    {
        Task<IGameGrain> StartNewGame();
        Task<Game> CurrentGame();
    }
}

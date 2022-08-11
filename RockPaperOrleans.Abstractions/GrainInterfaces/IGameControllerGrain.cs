namespace RockPaperOrleans.Abstractions.GrainInterfaces
{
    public interface IGameControllerGrain
    {
        Task<Game> StartNewGame();
        Task<Game> CurrentGame();
    }
}

namespace RockPaperOrleans.Abstractions.GrainInterfaces
{
    public interface IGameControllerGrain
    {
        Task<IGameGrain> StartNewGame();
        Task<Game> CurrentGame();
    }
}

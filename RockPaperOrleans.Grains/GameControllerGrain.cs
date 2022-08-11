using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using RockPaperOrleans.Abstractions;

namespace RockPaperOrleans.Grains
{
    public class GameControllerGrain : Grain, IGameControllerGrain
    {
        public GameControllerGrain(ILogger<GameControllerGrain> logger)
        {
            Logger = logger;
        }

        private ILogger<GameControllerGrain> Logger { get; set; }

        public Task<Game> CurrentGame()
        {
            throw new NotImplementedException();
        }

        public Task<IGameGrain> StartNewGame()
        {
            throw new NotImplementedException();
        }
    }
}

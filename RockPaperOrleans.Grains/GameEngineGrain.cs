using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using RockPaperOrleans.Abstractions;

namespace RockPaperOrleans.Grains
{
    public class GameEngineGrain : Grain, IGameEngineGrain
    {
        public GameEngineGrain(ILogger<GameEngineGrain> logger)
        {
            Logger = logger;
        }

        private ILogger<GameEngineGrain> Logger { get; set; }
        private IGameGrain CurrentGameGrain { get; set; }

        public async Task<Game> CurrentGame()
        {
            return await CurrentGameGrain.GetGame();
        }

        public Task<IGameGrain> StartNewGame()
        {
            CurrentGameGrain = GrainFactory.GetGrain<IGameGrain>(Guid.NewGuid());

            try
            {
                CurrentGameGrain.SelectPlayers();
            }
            catch(ArgumentException ex)
            {
                Logger.LogInformation(ex.Message);
            }
            catch
            {
                
            }
            
            return Task.FromResult(CurrentGameGrain);
        }

        public async Task<bool> IsGameComplete()
        {
            CurrentGameGrain = CurrentGameGrain ??= GrainFactory.GetGrain<IGameGrain>(Guid.NewGuid());

            var game = await CurrentGameGrain.GetGame();
            return (game.Rounds < game.Turns.Count);
        }
    }
}

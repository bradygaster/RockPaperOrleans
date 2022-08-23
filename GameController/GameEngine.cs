using Orleans;
using RockPaperOrleans.Abstractions;

namespace GameController
{
    public class GameEngine : BackgroundService
    {
        public GameEngine(IGrainFactory grainFactory, ILogger<GameEngine> logger)
        {
            GrainFactory = grainFactory;
            Logger = logger;
        }

        public IGrainFactory GrainFactory { get; set; }
        public ILogger<GameEngine> Logger { get; set; }
        public IGameGrain? CurrentGameGrain { get; set; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var newGame = () => CurrentGameGrain = GrainFactory.GetGrain<IGameGrain>(Guid.NewGuid());
            var delay = 250;

            while (!stoppingToken.IsCancellationRequested)
            {
                // start a new game if we don't have one yet
                if (CurrentGameGrain == null) newGame();

                var currentGame = await CurrentGameGrain.GetGame();

                // select players if they're unselected so far
                if (currentGame.Player1 == null && currentGame.Player2 == null)
                {
                    await CurrentGameGrain.SelectPlayers();
                }
                else
                {
                    if (currentGame.Rounds > currentGame.Turns.Count)
                    {
                        await CurrentGameGrain.Go();
                        await Task.Delay(delay);
                        await CurrentGameGrain.ScoreTurn();
                    }
                    else
                    {
                        await CurrentGameGrain.ScoreGame();
                        await Task.Delay(delay);
                        newGame();
                    }
                }

                await Task.Delay(delay);
            }
        }
    }
}

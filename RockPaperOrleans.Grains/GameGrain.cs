using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;
using RockPaperOrleans.Abstractions;

namespace RockPaperOrleans.Grains
{
    [CollectionAgeLimit(Minutes = 2)]
    public class GameGrain : Grain, IGameGrain
    {
        private IPersistentState<Game> Game { get; set; }
        private ILogger<GameGrain> Logger;

        public GameGrain(
            [PersistentState(nameof(GameGrain))] IPersistentState<Game> game,
            ILogger<GameGrain> logger)
        {
            Game = game;
            Logger = logger;
        }

        public Task<Game> GetGame() => Task.FromResult(Game.State);

        public async Task SetGame(Game game)
        {
            Game.State.Id = this.GetPrimaryKey();
            Game.State = game;
            await Game.WriteStateAsync();
        }

        public async Task SelectPlayers()
        {
            await Game.ReadStateAsync();
            Game.State.Id = this.GetPrimaryKey();

            Logger.LogInformation($"Getting the matchmaker for Game {Game.State.Id}");
            var matchmaker = GrainFactory.GetGrain<IMatchmakingGrain>(this.GetPrimaryKey());

            Logger.LogInformation($"Getting players for Game {Game.State.Id}");
            var players = await matchmaker.ChoosePlayers();

            if (players == null)
            {
                Logger.LogInformation("There aren't enough players in the lobby to field a game.");
            }
            else
            {
                Logger.LogInformation($"Players {players.Item1.Name} and {players.Item2.Name} selected for Game {Game.State.Id}.");

                // start the game
                Game.State.Player1 = players.Item1;
                Game.State.Player2 = players.Item2;
                Game.State.Started = DateTime.Now;
                await Game.WriteStateAsync();
            }
        }
    }
}

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

        public async Task<Game> GetGame()
        {
            //await Game.ReadStateAsync();
            return Game.State;
        }

        public async Task SetGame(Game game)
        {
            Game.State = game;
            await Game.WriteStateAsync();
        }

        public async Task SelectPlayers()
        {
            var game = await GetGame();

            Logger.LogInformation($"Getting the matchmaker for Game {Game.State.Id}");
            var matchmaker = GrainFactory.GetGrain<IMatchmakingGrain>(Guid.Empty);

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
                game.Player1 = players.Item1.Name;
                game.Player2 = players.Item2.Name;
                game.Started = DateTime.Now;
                await SetGame(game);

                // notify the players
                await GrainFactory
                        .GetGrain<IPlayerGrain>(players.Item1.Name)
                            .OpponentSelected(players.Item2);

                await GrainFactory
                        .GetGrain<IPlayerGrain>(players.Item2.Name)
                            .OpponentSelected(players.Item1);

            }
        }
    }
}

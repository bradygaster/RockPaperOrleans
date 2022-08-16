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

        public override Task OnActivateAsync()
        {
            Game.State.Id = this.GetGrainIdentity().PrimaryKey;

            return base.OnActivateAsync();
        }

        public Task<Game> GetGame()
            => Task.FromResult(Game.State);

        public async Task SetGame(Game game) 
            => Game.State = game;

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

        public async Task Go()
        {
            var player1Grain = GrainFactory.GetGrain<IPlayerGrain>(Game.State.Player1);
            var player2Grain = GrainFactory.GetGrain<IPlayerGrain>(Game.State.Player2);

            if (!(await player1Grain.IsPlayerOnline()
                && await player2Grain.IsPlayerOnline()))
                await Score();

            var player1Play = await player1Grain.Go();
            var player2Play = await player2Grain.Go();

            var turn = new Turn();
            turn.Throws.Add(new Throw { Play = player1Play, Player = Game.State.Player1 });
            turn.Throws.Add(new Throw { Play = player2Play, Player = Game.State.Player2 });
            turn.Winner = turn.ScoreTurn();

            Game.State.Turns.Add(turn);
            await SetGame(Game.State);

            await player1Grain.TurnComplete(turn);
            await player2Grain.TurnComplete(turn);
        }

        public async Task Score()
        {
            var player1Grain = GrainFactory.GetGrain<IPlayerGrain>(Game.State.Player1);
            var player2Grain = GrainFactory.GetGrain<IPlayerGrain>(Game.State.Player2);
            var player1 = await player1Grain.Get();
            var player2 = await player2Grain.Get();

            var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);

            if (await player1Grain.IsPlayerOnline())
                await lobbyGrain.Enter(player1);

            if (await player2Grain.IsPlayerOnline())
                await lobbyGrain.Enter(player2);

            var player1WinCount = Game.State.Turns.Count(x => x.Winner == player1.Name);
            var player2WinCount = Game.State.Turns.Count(x => x.Winner == player2.Name);

            Logger.LogInformation($"{player1.Name} won {player1WinCount} out of {Game.State.Rounds} rounds.");
            Logger.LogInformation($"{player2.Name} won {player2WinCount} out of {Game.State.Rounds} rounds.");

            if (player1WinCount > player2WinCount)
            {
                await player1Grain.RecordWin();
                await player2Grain.RecordLoss();
                Game.State.Winner = player1.Name;
                Logger.LogInformation($"{player1.Name} wins.");
            }
            if (player2WinCount > player1WinCount)
            {
                await player2Grain.RecordWin();
                await player1Grain.RecordLoss();
                Game.State.Winner = player2.Name;
                Logger.LogInformation($"{player2.Name} wins.");
            }
            if (player2WinCount == player1WinCount)
            {
                await player2Grain.RecordTie();
                await player1Grain.RecordTie();
                Game.State.Winner = "Tie";
                Logger.LogInformation($"{player1.Name} ties with {player2.Name}.");
            }

            await SetGame(Game.State);
        }
    }
}

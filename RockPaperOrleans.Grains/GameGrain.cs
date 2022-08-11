using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using RockPaperOrleans.Abstractions;

namespace RockPaperOrleans.Grains
{
    public class GameGrain : Grain, IGameGrain
    {
        private IPersistentState<Game> Game { get; set; }
        public Tuple<Player, Player> Players { get; private set; }
        private ILogger<GameGrain> Logger;

        public GameGrain(
            [PersistentState(nameof(GameGrain))]  IPersistentState<Game> game, 
            ILogger<GameGrain> logger)
        {
            Game = game;
            Logger = logger;
        }

        public async Task SelectPlayers()
        {
            var matchmaker = GrainFactory.GetGrain<IMatchmakingGrain>(Guid.Empty);
            var lobby = await matchmaker.GetPlayersInLobby();

            Players = await matchmaker.ChoosePlayers(lobby);

            // start the game
            Game.State.Player1 = Players.Item1;
            Game.State.Player2 = Players.Item2;
            Game.State.Started = DateTime.Now;
        }

        public async Task NotifyPlayers(Tuple<IStrategy, IStrategy> players)
        {
            var token = CancellationToken.None;

            var player1Play = await players.Item1.Play();
            var player2Play = await players.Item2.Play();
        }

        public Task SubmitPlay(Throw playerThrew)
        {
            throw new NotImplementedException();
        }

        public Task Score()
        {
            throw new NotImplementedException();
        }
    }
}

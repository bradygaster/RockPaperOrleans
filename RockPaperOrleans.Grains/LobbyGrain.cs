using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using RockPaperOrleans.Abstractions;

namespace RockPaperOrleans.Grains
{
    public class LobbyGrain : Grain, ILobbyGrain
    {
        private IPersistentState<List<Player>> Players { get; set; }
        public ILogger<LobbyGrain> Logger { get; set; }

        public LobbyGrain(
            [PersistentState(nameof(LobbyGrain))] IPersistentState<List<Player>> players, 
            ILogger<LobbyGrain> logger)
        {
            Players = players;
            Logger = logger;
        }

        public Task<List<Player>> GetPlayersInLobby() 
            => Task.FromResult(Players.State);

        public Task Enter(Player player)
        {
            if(!Players.State.Any(x => x.Name == player.Name))
            {
                Players.State.Add(player);
            }

            return Task.CompletedTask;
        }

        public Task Leave(Player player)
        {
            if (Players.State.Any(x => x.Name == player.Name))
            {
                Players.State.Remove(player);
            }

            return Task.CompletedTask;
        }
    }
}

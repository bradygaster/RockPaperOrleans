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

        public async Task Enter(Player player)
        {
            if(!Players.State.Any(x => x.Name == player.Name))
            {
                Logger.LogInformation($"{player.Name} has entered the lobby.");
                Players.State.Add(player);
                await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).LobbyUpdated(Players.State);
            }
        }

        public async Task Leave(Player player)
        {
            if (Players.State.Any(x => x.Name == player.Name))
            {
                Logger.LogInformation($"{player.Name} has left the lobby.");
                Players.State.RemoveAll(x => x.Name == player.Name);
                await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).LobbyUpdated(Players.State);
            }
        }
    }
}

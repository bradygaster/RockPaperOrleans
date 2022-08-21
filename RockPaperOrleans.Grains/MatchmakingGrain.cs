using Microsoft.Extensions.Logging;
using Orleans;
using RockPaperOrleans.Abstractions;

namespace RockPaperOrleans.Grains
{
    public class MatchmakingGrain : Grain, IMatchmakingGrain
    {
        public ILogger<MatchmakingGrain> Logger { get; set; }

        public MatchmakingGrain(ILogger<MatchmakingGrain> logger)
        {
            Logger = logger;
        }

        public async Task<Tuple<Player, Player>> ChoosePlayers()
        {
            var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);
            var lobby = await lobbyGrain.GetPlayersInLobby();

            Logger.LogInformation($"There are {lobby.Count} players in the lobby.");

            if (!(lobby.Count >= 2))
            {
                return null;
            }

            var players = lobby.OrderBy(x => Guid.NewGuid()).Take(2).ToArray();

            await lobbyGrain.Leave(players[0]);
            await lobbyGrain.Leave(players[1]);

            return new Tuple<Player, Player>(players[0], players[1]);
        }
    }
}

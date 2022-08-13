using Orleans;
using RockPaperOrleans.Abstractions;

namespace RockPaperOrleans.Grains
{
    public class MatchmakingGrain : Grain, IMatchmakingGrain
    {
        public async Task<Tuple<Player, Player>> ChoosePlayers()
        {
            var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);
            var lobby = await lobbyGrain.GetPlayersInLobby();

            if (!(lobby.Count >= 2))
            {
                return null;
            }

            var players = lobby.Take(2).ToArray();

            await lobbyGrain.Leave(players[0]);
            await lobbyGrain.Leave(players[1]);

            return new Tuple<Player, Player>(players[0], players[1]);
        }
    }
}

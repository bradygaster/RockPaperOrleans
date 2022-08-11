using Orleans;
using RockPaperOrleans.Abstractions;

namespace RockPaperOrleans.Grains
{
    public class MatchmakingGrain : Grain, IMatchmakingGrain
    {
        public async Task<Tuple<Player, Player>> ChoosePlayers(Player[] playersInQueue)
        {
            if (!(playersInQueue.Length >= 2))
            {
                throw new ArgumentException("The game requires two players.");
            }

            var players = playersInQueue.Take(2).ToArray();

            var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);

            await lobbyGrain.Leave(players[0]);
            await lobbyGrain.Leave(players[1]);

            return new Tuple<Player, Player>(players[0], players[1]);
        }

        public async Task<Player[]> GetPlayersInLobby()
        {
            var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);
            var players = await lobbyGrain.GetPlayersInLobby();
            return players.ToArray();
        }
    }
}

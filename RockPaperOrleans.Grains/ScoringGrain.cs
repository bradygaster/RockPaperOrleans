using Orleans;
using RockPaperOrleans.Abstractions;

namespace RockPaperOrleans.Grains
{
    public class ScoringGrain : Grain, IScoringGrain
    {
        public async Task AlertLoser(Player player)
        {
            var loser = GrainFactory.GetGrain<IPlayerGrain>(player.Name);
            await loser.RecordLoss();
        }

        public async Task AlertWinner(Player player)
        {
            var winner = GrainFactory.GetGrain<IPlayerGrain>(player.Name);
            await winner.RecordWin();
        }
    }
}

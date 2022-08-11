using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using RockPaperOrleans.Abstractions;

namespace RockPaperOrleans.Grains
{
    public class PlayerGrain : Grain, IPlayerGrain
    {
        public PlayerGrain(
            [PersistentState(nameof(PlayerGrain))] IPersistentState<Player> player,
            ILogger<PlayerGrain> logger)
        {
            Player = player;
            Logger = logger;
        }

        private IPersistentState<Player> Player { get; set; }
        public ILogger<PlayerGrain> Logger { get; set; }

        public Task<Player> Get() => Task.FromResult(Player.State);

        public Task Play(Play play)
        {
            throw new NotImplementedException();
        }

        public async Task RecordLoss()
        {
            Player.State.LossCount += 1;
            await Player.WriteStateAsync();
        }

        public async Task RecordWin()
        {
            Player.State.WinCount += 1;
            await Player.WriteStateAsync();
        }
    }
}

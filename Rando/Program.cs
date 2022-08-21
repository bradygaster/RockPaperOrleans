using RockPaperOrleans;
using RockPaperOrleans.Abstractions;

await Task.Delay(20000); // for debugging, give the silo time to warm up

IHost host = Host.CreateDefaultBuilder(args)
    .UseOrleans((context, siloBuilder) =>
    {
        siloBuilder
            .PlayRockPaperOrleans(context.Configuration)
            .EnlistPlayer<Rando>()
            .EnlistPlayer<SlowRando>();
    })
    .Build();

await host.RunAsync();

public class Rando : PlayerBase
{
    public Rando(ILogger<Rando> logger) : base(logger) { }  
    public override Task<Play> Go() => Task.FromResult((Play)Random.Shared.Next(0, 3));
}

// simulate a player taking a few seconds to run
public class SlowRando : PlayerBase
{
    public SlowRando(ILogger<Rando> logger) : base(logger) { }
    public override async Task<Play> Go()
    {
        await Task.Delay(Random.Shared.Next(250, 1000));
        return (Play)Random.Shared.Next(0, 3);
    }
}
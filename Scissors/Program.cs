using RockPaperOrleans;
using RockPaperOrleans.Abstractions;

await Task.Delay(30000); // for debugging, give the silo time to warm up

IHost host = Host.CreateDefaultBuilder(args)
    .UseOrleans((context, siloBuilder) =>
    {
        siloBuilder
            .PlayRockPaperOrleans(context.Configuration)
            .EnlistPlayer<AlwaysScissors>();
    })
    .Build();

await host.RunAsync();

public class AlwaysScissors : PlayerBase
{
    public AlwaysScissors(ILogger<AlwaysScissors> logger) : base(logger) { }

    public override Task<Play> Go()
        => Task.FromResult(Play.Scissors);
}
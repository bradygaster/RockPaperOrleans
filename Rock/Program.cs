using RockPaperOrleans;
using RockPaperOrleans.Abstractions;

await Task.Delay(30000); // for debugging, give the silo time to warm up

IHost host = Host.CreateDefaultBuilder(args)
    .UseOrleans((context, siloBuilder) =>
    {
        siloBuilder.EnlistPlayer<AlwaysRock>(context.Configuration);
    })
    .Build();

await host.RunAsync();

public class AlwaysRock : PlayerBase
{
    public AlwaysRock(ILogger<AlwaysRock> logger) : base(logger) { }

    public override Task<Play> Go()
    {
        return Task.FromResult(Play.Rock);
    }
}
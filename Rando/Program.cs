using RockPaperOrleans;
using RockPaperOrleans.Abstractions;

await Task.Delay(30000); // for debugging, give the silo time to warm up

IHost host = Host.CreateDefaultBuilder(args)
    .UseOrleans((context, siloBuilder) =>
    {
        siloBuilder.EnlistPlayer<Rando>(context.Configuration);
    })
    .Build();

await host.RunAsync();

public class Rando : PlayerBase
{
    public Rando(ILogger<Rando> logger) : base(logger) { }  

    Play LastPlay = Play.Rock;

    public override Task<Play> Go()
    {
        LastPlay = (Play)Random.Shared.Next(0, 3);
        Logger.LogInformation($"Rando throws {LastPlay}.");
        return Task.FromResult(LastPlay);
    }
}
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddKeyedRedisClient("redis");
builder.UseOrleans(siloBuilder =>
{
    if (builder.Environment.IsDevelopment())
    {
        siloBuilder.ConfigureEndpoints(
            Random.Shared.Next(10_000, 50_000),
            Random.Shared.Next(10_000, 50_000)
        );
    }

    siloBuilder
        .EnlistPlayer<RoundRobin>();
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();

public class RoundRobin : PlayerBase
{
    public RoundRobin(ILogger<RoundRobin> logger) : base(logger) { }

    private static readonly Play[] availablePlays = new[] { Play.Paper, Play.Rock, Play.Scissors };
    private static int index = -1;

    public override Task<Play> Go()
    {
        index++;
        if (index == availablePlays.Length)
        {
            index = 0;
        }

        return Task.FromResult(availablePlays[index]);
    }
}

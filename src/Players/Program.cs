using RockPaperOrleans;
using RockPaperOrleans.Abstractions;

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
        .EnlistPlayer<AlwaysPaper>()
        .EnlistPlayer<AlwaysRock>()
        .EnlistPlayer<AlwaysScissors>();
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();

public class AlwaysPaper : PlayerBase
{
    public AlwaysPaper(ILogger<AlwaysPaper> logger) : base(logger) { }

    public override Task<Play> Go()
        => Task.FromResult(Play.Paper);
}

public class AlwaysRock : PlayerBase
{
    public AlwaysRock(ILogger<AlwaysRock> logger) : base(logger) { }

    public override Task<Play> Go()
        => Task.FromResult(Play.Rock);
}

public class AlwaysScissors : PlayerBase
{
    public AlwaysScissors(ILogger<AlwaysScissors> logger) : base(logger) { }

    public override Task<Play> Go()
        => Task.FromResult(Play.Scissors);
}

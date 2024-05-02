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
        .EnlistPlayer<Rando>()
        .EnlistPlayer<SlowRando>()
        .EnlistPlayer<CaptainObvious>();
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();

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


public class CaptainObvious : PlayerBase
{
    private Player _opponent;

    public CaptainObvious(ILogger<CaptainObvious> logger) : base(logger) { }

    public override Task OnOpponentSelected(Player player, Player opponent)
    {
        _opponent = opponent;
        return base.OnOpponentSelected(player, opponent);
    }

    public override Task<Play> Go()
    {
        var result = (_opponent) switch
        {
            Player _ when _opponent.Name.ToLower().Contains("scissors") => Play.Rock,
            Player _ when _opponent.Name.ToLower().Contains("rock") => Play.Paper,
            Player _ when _opponent.Name.ToLower().Contains("paper") => Play.Scissors,
            _ => (Play)Random.Shared.Next(0, 3)
        };

        return Task.FromResult(result);
    }
}

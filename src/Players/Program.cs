var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRockPaperOrleans(siloBuilder => 
    siloBuilder.AddPlayer<AlwaysPaper>()
               .AddPlayer<AlwaysRock>()
               .AddPlayer<AlwaysScissors>());

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

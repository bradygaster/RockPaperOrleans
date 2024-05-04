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
    public override Task<Play> Go()
        => Task.FromResult(Play.Paper);
}

public class AlwaysRock : PlayerBase
{
    public override Task<Play> Go()
        => Task.FromResult(Play.Rock);
}

public class AlwaysScissors : PlayerBase
{
    public override Task<Play> Go()
        => Task.FromResult(Play.Scissors);
}

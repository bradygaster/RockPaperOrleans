var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRockPaperOrleans(siloBuilder => 
    siloBuilder.AddPlayer<AlwaysPaper>()
               .AddPlayer<AlwaysRock>()
               .AddPlayer<AlwaysScissors>());

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();

public class AlwaysPaper : IPlayerGrain
{
    public Task<Play> Go(Player opponent) => Task.FromResult(Play.Paper);
}

public class AlwaysRock : IPlayerGrain
{
    public Task<Play> Go(Player opponent) => Task.FromResult(Play.Rock);
}

public class AlwaysScissors : IPlayerGrain
{
    public Task<Play> Go(Player opponent) => Task.FromResult(Play.Scissors);
}

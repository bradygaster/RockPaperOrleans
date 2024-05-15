var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRockPaperOrleans(siloBuilder => siloBuilder.AddPlayer<Rando>());

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();

public class Rando : IPlayerGrain
{
    public Task<Play> Go(Player opponent) => Task.FromResult((Play)Random.Shared.Next(0, 3));
}

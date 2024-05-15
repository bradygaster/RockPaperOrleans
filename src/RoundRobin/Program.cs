var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRockPaperOrleans(siloBuilder => siloBuilder.AddPlayer<RoundRobin>());

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();

public class RoundRobin : IPlayerGrain
{
    private static readonly Play[] availablePlays = [Play.Paper, Play.Rock, Play.Scissors];
    private static int index = -1;

    public Task<Play> Go(Player opponent)
    {
        index = (index + 1) % availablePlays.Length;
        return Task.FromResult(availablePlays[index]);
    }
}

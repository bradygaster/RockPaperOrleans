var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRockPaperOrleans(siloBuilder =>
    siloBuilder.AddPlayer<RoundRobin>());

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();

public class RoundRobin : PlayerBase
{
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

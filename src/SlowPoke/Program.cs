var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRockPaperOrleans(siloBuilder => siloBuilder.AddPlayer<SlowPoke>());

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();

public class SlowPoke : IPlayerGrain
{
    public async Task<Play> Go(Player opponent)
    {
        // Simulate a slow player
        await Task.Delay(Random.Shared.Next(500, 1500));
        return (Play)Random.Shared.Next(0, 3);
    }
}

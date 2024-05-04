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
        // simlulate a slow player
        await Task.Delay(Random.Shared.Next(500, 3000));
        return (Play)Random.Shared.Next(0, 3);
    }
}

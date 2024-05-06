var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRockPaperOrleans(siloBuilder => siloBuilder.AddPlayer<MrPresumptive>());

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();

public class MrPresumptive : IPlayerGrain
{
    public Task<Play> Go(Player opponent)
    {
        var result = opponent.Name switch
        {
            { } name when name.Contains("scissors", StringComparison.OrdinalIgnoreCase) => Play.Rock,
            { } name when name.Contains("rock", StringComparison.OrdinalIgnoreCase) => Play.Paper,
            { } name when name.Contains("paper", StringComparison.OrdinalIgnoreCase) => Play.Scissors,
            _ => (Play)Random.Shared.Next(0, 3)
        };

        return Task.FromResult(result);
    }
}

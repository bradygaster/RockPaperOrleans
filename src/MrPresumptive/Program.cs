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
        var result = (opponent) switch
        {
            Player _ when opponent.Name.ToLower().Contains("scissors") => Play.Rock,
            Player _ when opponent.Name.ToLower().Contains("rock") => Play.Paper,
            Player _ when opponent.Name.ToLower().Contains("paper") => Play.Scissors,
            _ => (Play)Random.Shared.Next(0, 3)
        };

        return Task.FromResult(result);
    }
}

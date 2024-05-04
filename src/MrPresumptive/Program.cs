var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRockPaperOrleans(siloBuilder => siloBuilder.AddPlayer<MrPresumptive>());

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();

public class MrPresumptive : PlayerBase
{
    private Player _opponent;

    public override Task<Play> Go()
    {
        var result = (_opponent) switch
        {
            Player _ when _opponent.Name.ToLower().Contains("scissors") => Play.Rock,
            Player _ when _opponent.Name.ToLower().Contains("rock") => Play.Paper,
            Player _ when _opponent.Name.ToLower().Contains("paper") => Play.Scissors,
            _ => (Play)Random.Shared.Next(0, 3)
        };

        return Task.FromResult(result);
    }
}

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRockPaperOrleans(siloBuilder => siloBuilder.AddPlayer<MrPresumptive>());

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();

public class MrPresumptive : PlayerBase
{
    private Player _opponent;

    public MrPresumptive(ILogger<MrPresumptive> logger) : base(logger) { }

    public override Task OnOpponentSelected(Player player, Player opponent)
    {
        _opponent = opponent;
        return base.OnOpponentSelected(player, opponent);
    }

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

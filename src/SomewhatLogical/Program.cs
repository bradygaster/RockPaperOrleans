var builder = WebApplication.CreateBuilder(args);

builder.AddRockPaperOrleans(siloBuilder => siloBuilder.AddPlayer<SomewhatLogical>());

var app = builder.Build();

app.Run();

public class SomewhatLogical : BasePlayerGrain
{
    public override Task<Play> Go(Player opponent) => opponent.Name switch
    {
        "AlwaysPaper" => Task.FromResult(Play.Scissors),
        "AlwaysRock" => Task.FromResult(Play.Paper),
        "AlwaysScissors" => Task.FromResult(Play.Rock),
        "NeverPaper" => Randomize([Play.Rock, Play.Paper]),
        "NeverRock" => Randomize([Play.Rock, Play.Scissors]),
        "NeverScissors" => Randomize([Play.Paper, Play.Scissors]),
        "RoundRobin" => HandleRoundRobin(),
        _ => Randomize([Play.Paper, Play.Rock, Play.Scissors])
    };

    public override Task OnTurnCompleted(Turn turn)
    {
        if (turn.Throws.Any(x => x.Player == "RoundRobin")) lastRoundRobinPlay = turn.Throws.First(x => x.Player == "RoundRobin").Play;
        return Task.CompletedTask;
    }

    private Task<Play> Randomize(Play[] plays) => Task.FromResult((Play)Random.Shared.Next(0, plays.Length));

    private Play lastRoundRobinPlay = Play.Scissors;
    private Task<Play> HandleRoundRobin() => Task.FromResult(lastRoundRobinPlay);
}

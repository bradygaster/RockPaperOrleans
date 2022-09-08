using RockPaperOrleans;
using RockPaperOrleans.Abstractions;

await Task.Delay(20000); // for debugging, give the silo time to warm up

IHost host = Host.CreateDefaultBuilder(args)
    .UseOrleans((context, siloBuilder) =>
    {
        siloBuilder
            .PlayRockPaperOrleans(context.Configuration)
            .EnlistPlayer<Rando>()
            .EnlistPlayer<SlowRando>()
            .EnlistPlayer<CaptainObvious>()
            .EnlistPlayer<NeverPaper>()
            .EnlistPlayer<NeverRock>()
            .EnlistPlayer<NeverScissors>();
    })
    .ConfigureServices((services) =>
    {
        services.AddWorkerAppApplicationInsights("Rando Silo");
    })
    .Build();

await host.RunAsync();

public class Rando : PlayerBase
{
    public Rando(ILogger<Rando> logger) : base(logger) { }  
    public override Task<Play> Go() => Task.FromResult((Play)Random.Shared.Next(0, 3));
}

// simulate a player taking a few seconds to run
public class SlowRando : PlayerBase
{
    public SlowRando(ILogger<Rando> logger) : base(logger) { }
    public override async Task<Play> Go()
    {
        await Task.Delay(Random.Shared.Next(250, 1000));
        return (Play)Random.Shared.Next(0, 3);
    }
}


public class CaptainObvious : PlayerBase
{
    private Player _opponent;

    public CaptainObvious(ILogger<CaptainObvious> logger) : base(logger) { }

    public override Task OnOpponentSelected(Player player, Player opponent)
    {
        _opponent = opponent;
        return base.OnOpponentSelected(player, opponent);
    }

    public override Task<Play> Go()
    {
        // start with random
        var result = (Play)Random.Shared.Next(0, 3);

        if (_opponent.Name.ToLower().Contains("scissors"))
        {
            result = Play.Rock;
        }
        if (_opponent.Name.ToLower().Contains("rock"))
        {
            result = Play.Paper;
        }
        if (_opponent.Name.ToLower().Contains("paper"))
        {
            result = Play.Scissors;
        }

        return Task.FromResult(result);
    }
}

public class NeverPaper : PlayerBase
{
    private Player _opponent;

    private static readonly Play[] availablePlays = new[] { Play.Rock, Play.Scissors };

    public NeverPaper(ILogger<NeverPaper> logger) : base(logger) { }

    public override Task OnOpponentSelected(Player player, Player opponent)
    {
        _opponent = opponent;
        return base.OnOpponentSelected(player, opponent);
    }

    public override Task<Play> Go()
    {
        var result = availablePlays[Random.Shared.Next(0, 1)];

        return Task.FromResult(result);
    }
}

public class NeverRock : PlayerBase
{
    private Player _opponent;

    private static readonly Play[] availablePlays = new[] { Play.Paper, Play.Scissors };

    public NeverRock(ILogger<NeverRock> logger) : base(logger) { }

    public override Task OnOpponentSelected(Player player, Player opponent)
    {
        _opponent = opponent;
        return base.OnOpponentSelected(player, opponent);
    }

    public override Task<Play> Go()
    {
        var result = availablePlays[Random.Shared.Next(0, 1)];

        return Task.FromResult(result);
    }
}

public class NeverScissors : PlayerBase
{
    private Player _opponent;

    private static readonly Play[] availablePlays = new[] { Play.Paper, Play.Rock };

    public NeverScissors(ILogger<NeverScissors> logger) : base(logger) { }

    public override Task OnOpponentSelected(Player player, Player opponent)
    {
        _opponent = opponent;
        return base.OnOpponentSelected(player, opponent);
    }

    public override Task<Play> Go()
    {
        var result = availablePlays[Random.Shared.Next(0, 1)];

        return Task.FromResult(result);
    }
}

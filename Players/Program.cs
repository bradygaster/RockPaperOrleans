using RockPaperOrleans;
using RockPaperOrleans.Abstractions;

await Task.Delay(20000); // for debugging, give the silo time to warm up

IHost host = Host.CreateDefaultBuilder(args)
    .UseOrleans((context, siloBuilder) =>
    {
        siloBuilder
            .PlayRockPaperOrleans(context.Configuration)
            .EnlistPlayer<CaptainObvious>()
            .EnlistPlayer<AlwaysPaper>()
            .EnlistPlayer<AlwaysRock>()
            .EnlistPlayer<AlwaysScissors>();
    })
    .ConfigureServices((services) =>
    {
        services.AddWorkerAppApplicationInsights("Players Silo");
    })
    .Build();

await host.RunAsync();

public class AlwaysPaper : PlayerBase
{
    public AlwaysPaper(ILogger<AlwaysPaper> logger) : base(logger) { }

    public override Task<Play> Go()
        => Task.FromResult(Play.Paper);
}

public class AlwaysRock : PlayerBase
{
    public AlwaysRock(ILogger<AlwaysRock> logger) : base(logger) { }

    public override Task<Play> Go()
        => Task.FromResult(Play.Rock);
}

public class AlwaysScissors : PlayerBase
{
    public AlwaysScissors(ILogger<AlwaysScissors> logger) : base(logger) { }

    public override Task<Play> Go()
        => Task.FromResult(Play.Scissors);
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

        if(_opponent.Name.ToLower().Contains("scissors"))
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

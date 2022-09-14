using RockPaperOrleans;
using RockPaperOrleans.Abstractions;

await Task.Delay(20000); // for debugging, give the silo time to warm up

IHost host = Host.CreateDefaultBuilder(args)
	.UseOrleans((context, siloBuilder) =>
	{
		siloBuilder
			.PlayRockPaperOrleans(context.Configuration)
			.EnlistPlayer<NeverPaper>()
			.EnlistPlayer<NeverRock>()
			.EnlistPlayer<NeverScissors>()
			.EnlistPlayer<RockPaperScissors>()
			.EnlistPlayer<RoundRobin>()
			.EnlistPlayer<DontWorryIGotThis>();
	})
	.ConfigureServices((services) =>
	{
		services.AddWorkerAppApplicationInsights("Rascals Silo");
	})
	.Build();

await host.RunAsync();

public class RockPaperScissors : PlayerBase
{
	public RockPaperScissors(ILogger<RockPaperScissors> logger) : base(logger) { }

	public override Task<Play> Go() => Task.FromResult((Play)Random.Shared.Next(0, 3));
}

public class RoundRobin : PlayerBase
{
	public RoundRobin(ILogger<RoundRobin> logger) : base(logger) { }

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

public class DontWorryIGotThis : PlayerBase
{
	public DontWorryIGotThis(ILogger<DontWorryIGotThis> logger) : base(logger) { }

	private static readonly Play[] roundRobinPlays = new[] 
	{
		Play.Paper, Play.Rock, Play.Scissors, 
		Play.Rock, Play.Rock, Play.Rock,
		Play.Scissors, Play.Scissors, Play.Scissors, 
		Play.Paper, Play.Paper, Play.Paper,
		Play.Rock, Play.Rock, Play.Paper,
		Play.Paper, Play.Paper, Play.Rock,
		Play.Paper, Play.Paper, Play.Scissors,
		Play.Rock, Play.Rock, Play.Paper,
		Play.Rock, Play.Rock, Play.Scissors,
		Play.Scissors, Play.Scissors, Play.Paper,
		Play.Scissors, Play.Scissors, Play.Rock
	};
	
	private static int roundRobinIndex = -1;

	private static readonly Play[] paperPlays = new[] { Play.Rock, Play.Scissors };
	private static readonly Play[] rockPlays = new[] { Play.Paper, Play.Scissors };
	private static readonly Play[] scissorsPlays = new[] { Play.Paper, Play.Rock };
	

	private Player _opponent;

	public override Task OnOpponentSelected(Player player, Player opponent)
	{
		_opponent = opponent;
		return base.OnOpponentSelected(player, opponent);
	}

	public override Task<Play> Go()
	{
		var result = (_opponent) switch
		{
			Player _ when _opponent.Name.ToLower().Contains("paper") => paperPlays[Random.Shared.Next(0, 1)],
			Player _ when _opponent.Name.ToLower().Contains("rock") => rockPlays[Random.Shared.Next(0, 1)],
			Player _ when _opponent.Name.ToLower().Contains("scissors") => scissorsPlays[Random.Shared.Next(0, 1)],
			_ => RoundRobinPlay()
		};

		return Task.FromResult(result);
	}

	private Play RoundRobinPlay()
	{
		roundRobinIndex++;
		if (roundRobinIndex == roundRobinPlays.Length)
		{
			roundRobinIndex = 0;
		}

		return roundRobinPlays[roundRobinIndex];
	}
}

public class NeverPaper : PlayerBase
{
	private static readonly Play[] availablePlays = new[] { Play.Rock, Play.Scissors };

	public NeverPaper(ILogger<NeverPaper> logger) : base(logger) { }

	public override Task<Play> Go()
	{
		var result = availablePlays[Random.Shared.Next(0, 1)];

		return Task.FromResult(result);
	}
}

public class NeverRock : PlayerBase
{
	private static readonly Play[] availablePlays = new[] { Play.Paper, Play.Scissors };

	public NeverRock(ILogger<NeverRock> logger) : base(logger) { }

	public override Task<Play> Go()
	{
		var result = availablePlays[Random.Shared.Next(0, 1)];

		return Task.FromResult(result);
	}
}

public class NeverScissors : PlayerBase
{
	private static readonly Play[] availablePlays = new[] { Play.Paper, Play.Rock };

	public NeverScissors(ILogger<NeverScissors> logger) : base(logger) { }


	public override Task<Play> Go()
	{
		var result = availablePlays[Random.Shared.Next(0, 1)];

		return Task.FromResult(result);
	}
}

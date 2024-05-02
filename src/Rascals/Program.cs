using RockPaperOrleans;
using RockPaperOrleans.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddKeyedRedisClient("redis");
builder.UseOrleans(siloBuilder =>
{
    if (builder.Environment.IsDevelopment())
    {
        siloBuilder.ConfigureEndpoints(
            Random.Shared.Next(10_000, 50_000),
            Random.Shared.Next(10_000, 50_000)
        );
    }

	siloBuilder
		.EnlistPlayer<RockPaperScissors>()
		.EnlistPlayer<NeverScissors>()
		.EnlistPlayer<NeverRock>()
		.EnlistPlayer<NeverPaper>()
        .EnlistPlayer<RoundRobin>();
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();

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

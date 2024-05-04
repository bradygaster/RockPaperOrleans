var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRockPaperOrleans(siloBuilder =>
    siloBuilder.AddPlayer<NeverPaper>()
               .AddPlayer<NeverRock>()
               .AddPlayer<NeverScissors>());

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();

public class NeverPaper : PlayerBase
{
	private static readonly Play[] availablePlays = new[] { Play.Rock, Play.Scissors };

	public override Task<Play> Go()
	{
		var result = availablePlays[Random.Shared.Next(0, 1)];

		return Task.FromResult(result);
	}
}

public class NeverRock : PlayerBase
{
	private static readonly Play[] availablePlays = new[] { Play.Paper, Play.Scissors };

	public override Task<Play> Go()
	{
		var result = availablePlays[Random.Shared.Next(0, 1)];

		return Task.FromResult(result);
	}
}

public class NeverScissors : PlayerBase
{
	private static readonly Play[] availablePlays = new[] { Play.Paper, Play.Rock };

	public override Task<Play> Go()
	{
		var result = availablePlays[Random.Shared.Next(0, 1)];

		return Task.FromResult(result);
	}
}

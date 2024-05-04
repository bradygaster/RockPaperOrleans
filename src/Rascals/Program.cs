var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRockPaperOrleans(siloBuilder =>
    siloBuilder.AddPlayer<NeverPaper>()
               .AddPlayer<NeverRock>()
               .AddPlayer<NeverScissors>());

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();

public class NeverPaper : IPlayerGrain
{
	private static readonly Play[] availablePlays = new[] { Play.Rock, Play.Scissors };

	public Task<Play> Go(Player opponent)
	{
		var result = availablePlays[Random.Shared.Next(0, 1)];

		return Task.FromResult(result);
	}
}

public class NeverRock : IPlayerGrain
{
	private static readonly Play[] availablePlays = new[] { Play.Paper, Play.Scissors };

	public Task<Play> Go(Player opponent)
	{
		var result = availablePlays[Random.Shared.Next(0, 1)];

		return Task.FromResult(result);
	}
}

public class NeverScissors : IPlayerGrain
{
	private static readonly Play[] availablePlays = new[] { Play.Paper, Play.Rock };

	public Task<Play> Go(Player opponent)
	{
		var result = availablePlays[Random.Shared.Next(0, 1)];

		return Task.FromResult(result);
	}
}

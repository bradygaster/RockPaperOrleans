var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRockPaperOrleans(siloBuilder =>
    siloBuilder.AddPlayer<NeverPaper>()
               .AddPlayer<NeverRock>()
               .AddPlayer<NeverScissors>());

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();

public class NeverPaper : BasePlayerGrain
{
	private static readonly Play[] availablePlays = [Play.Rock, Play.Scissors];

	public override Task<Play> Go(Player opponent)
	{
		var result = availablePlays[Random.Shared.Next(0, availablePlays.Length)];

		return Task.FromResult(result);
	}
}

public class NeverRock : BasePlayerGrain
{
	private static readonly Play[] availablePlays = [Play.Paper, Play.Scissors];

	public override Task<Play> Go(Player opponent)
	{
		var result = availablePlays[Random.Shared.Next(0, availablePlays.Length)];

		return Task.FromResult(result);
	}
}

public class NeverScissors : BasePlayerGrain
{
	private static readonly Play[] availablePlays = [Play.Paper, Play.Rock];

	public override Task<Play> Go(Player opponent)
	{
		var result = availablePlays[Random.Shared.Next(0, availablePlays.Length)];

		return Task.FromResult(result);
	}
}

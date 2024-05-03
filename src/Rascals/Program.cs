var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddKeyedAzureTableClient("clustering");
builder.AddKeyedAzureBlobClient("grainstorage");
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
		.EnlistPlayer<NeverScissors>()
		.EnlistPlayer<NeverRock>()
		.EnlistPlayer<NeverPaper>();
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();

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

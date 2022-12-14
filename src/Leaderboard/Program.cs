using Leaderboard.Hubs;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using MudBlazor.Services;
using Orleans;
using RockPaperOrleans.Abstractions;

await Task.Delay(10000); // for debugging, give the silo time to warm up

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseOrleans((context, siloBuilder) =>
{
    siloBuilder
        .PlayRockPaperOrleans(context.Configuration);
});

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.AddWebAppApplicationInsights("Leaderboard");
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<ILeaderboardGrainObserver, LeaderboardObserver>();
builder.Services.AddHostedService<LeaderboardObserverWorker>();
builder.Services.AddSignalR();
builder.Services.AddMudServices();

var app = builder.Build();
app.UseStaticFiles();
app.UseForwardedHeaders();
app.UseRouting();
app.MapBlazorHub();
app.MapHub<LeaderboardHub>("/hubs/leaderboard");
app.MapFallbackToPage("/_Host");

app.Run();

public class LeaderboardObserverWorker : IHostedService
{
    public ILeaderboardGrainObserver LeaderboardObserver { get; }
    public IGrainFactory GrainFactory { get; set; }
    public ILeaderboardGrain? Leaderboard { get; private set; }

    public LeaderboardObserverWorker(ILeaderboardGrainObserver leaderboardObserver,
        IGrainFactory grainFactory)
    {
        LeaderboardObserver = leaderboardObserver;
        GrainFactory = grainFactory;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Leaderboard = GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
        var reference = await GrainFactory.CreateObjectReference<ILeaderboardGrainObserver>(LeaderboardObserver);
        await Leaderboard.Subscribe(reference);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Leaderboard = GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
        var reference = await GrainFactory.CreateObjectReference<ILeaderboardGrainObserver>(LeaderboardObserver);
        await Leaderboard.UnSubscribe(reference);
    }
}

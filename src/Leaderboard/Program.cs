using Leaderboard.Hubs;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRockPaperOrleans();

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<ILeaderboardGrainObserver, LeaderboardObserver>();
builder.Services.AddHostedService<LeaderboardObserverWorker>();
builder.Services.AddSignalR();
builder.Services.AddMudServices();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseStaticFiles();
app.UseForwardedHeaders();
app.UseRouting();
app.MapBlazorHub();
app.MapHub<LeaderboardHub>("/hubs/leaderboard");
app.MapFallbackToPage("/_Host");

app.Run();

public class LeaderboardObserverWorker(ILeaderboardGrainObserver leaderboardObserver,
        IGrainFactory grainFactory,
        ILogger<LeaderboardObserverWorker> logger) : IHostedService
{
    private readonly ILogger<LeaderboardObserverWorker> logger = logger;
    public ILeaderboardGrainObserver LeaderboardObserver { get; } = leaderboardObserver;
    public IGrainFactory GrainFactory { get; set; } = grainFactory;
    public ILeaderboardGrain? Leaderboard { get; private set; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            try
            {
                Leaderboard = GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
                var reference = GrainFactory.CreateObjectReference<ILeaderboardGrainObserver>(LeaderboardObserver);
                await Leaderboard.Subscribe(reference);
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RPO: LeaderboardObserverWorker error:");
                await Task.Delay(1000);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Leaderboard = GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
        var reference = GrainFactory.CreateObjectReference<ILeaderboardGrainObserver>(LeaderboardObserver);
        await Leaderboard.UnSubscribe(reference);
    }
}

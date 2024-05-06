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

public class LeaderboardObserverWorker(
    ILeaderboardGrainObserver leaderboardObserver,
    IGrainFactory grainFactory,
    ILogger<LeaderboardObserverWorker> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            try
            {
                var leaderboard = grainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
                var reference = grainFactory.CreateObjectReference<ILeaderboardGrainObserver>(leaderboardObserver);
                await leaderboard.Subscribe(reference);
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RPO: LeaderboardObserverWorker error.");
                await Task.Delay(1000, cancellationToken);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var leaderboard = grainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
        var reference = grainFactory.CreateObjectReference<ILeaderboardGrainObserver>(leaderboardObserver);
        await leaderboard.UnSubscribe(reference);
    }
}

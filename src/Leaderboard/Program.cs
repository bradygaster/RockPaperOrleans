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
    ILogger<LeaderboardObserverWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var leaderboard = grainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
        var reference = grainFactory.CreateObjectReference<ILeaderboardGrainObserver>(leaderboardObserver);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await leaderboard.Subscribe(reference);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RPO: LeaderboardObserverWorker error.");
                if (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }

        await leaderboard.UnSubscribe(reference);
    }
}

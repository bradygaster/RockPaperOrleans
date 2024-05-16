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
builder.Services.AddSingleton<LeaderboardObserverWorker>();
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

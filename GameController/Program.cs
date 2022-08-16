using Orleans;
using GameController;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans((context, siloBuilder) =>
{
    siloBuilder
        .UseDashboard(dashboardOptions => dashboardOptions.HostSelf = false)
        .CreateGameEngine(context.Configuration);
});

builder.Services.AddServicesForSelfHostedDashboard();
builder.Services.AddHostedService<GameEngine>();

var app = builder.Build();
app.UseOrleansDashboard();

app.Run();



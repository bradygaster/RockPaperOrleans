using Orleans;
using GameController;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseOrleans((context, siloBuilder) =>
    {
        siloBuilder
            .PlayRockPaperOrleans(context.Configuration)
            .UseDashboard(dashboardOptions => dashboardOptions.HostSelf = false);
    });

builder.Services.AddWebAppApplicationInsights("Game Controller");
builder.Services.AddServicesForSelfHostedDashboard();
builder.Services.AddHostedService<GameEngine>();

var app = builder.Build();
app.UseOrleansDashboard();

app.Run();



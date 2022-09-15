using Orleans;
using GameController;
using Orleans.Hosting;
using static Orleans.Hosting.OrleansOnAzureConfiguration;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseOrleans((context, siloBuilder) =>
    {
        siloBuilder
            .PlayRockPaperOrleans(context.Configuration)
            .UseDashboard(dashboardOptions => {
                dashboardOptions.HostSelf = false;
                dashboardOptions.HideTrace = true;
            });

        if (context.Configuration.GetValue<string>(EnvironmentVariableNames.ApplicationInsights)
                is { Length: > 0 } instrumentationKey)
        {
            siloBuilder.AddApplicationInsightsTelemetryConsumer(instrumentationKey);
        }
    });

builder.Services.AddWebAppApplicationInsights("Game Controller");
builder.Services.AddServicesForSelfHostedDashboard();
builder.Services.AddHostedService<GameEngine>();

var app = builder.Build();
app.UseOrleansDashboard();

app.Run();



using Orleans;
using Orleans.Hosting;
using Silo;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans((context, siloBuilder) =>
{
    siloBuilder
        .UseDashboard(dashboardOptions => dashboardOptions.HostSelf = false)
        .HostInAzure(context.Configuration)
            .UseCosmosDbClustering()
            .UseCosmosDbGrainStorage();
});

builder.Services.AddServicesForSelfHostedDashboard();
builder.Services.AddHostedService<GameEngine>();

var app = builder.Build();
app.UseOrleansDashboard();

app.Run();



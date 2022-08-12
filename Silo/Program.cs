using Orleans;
using Orleans.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans((context, siloBuilder) =>
{
    siloBuilder
        .ConfigureApplicationParts(options => options.AddFromApplicationBaseDirectory())
        .UseDashboard(dashboardOptions => dashboardOptions.HostSelf = false)
        .HostInAzure(context.Configuration)
            .UseCosmosDbClustering()
            .UseCosmosDbGrainStorage();
});

builder.Services.AddServicesForSelfHostedDashboard();

var app = builder.Build();
app.UseOrleansDashboard();

app.Run();
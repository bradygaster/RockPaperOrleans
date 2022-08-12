using Orleans;
using Orleans.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder
        .ConfigureApplicationParts(options => options.AddFromApplicationBaseDirectory())
        .UseDashboard(dashboardOptions => dashboardOptions.HostSelf = false)
        .HostInAzure()
            .UseCosmosDbClustering()
            .UseCosmosDbGrainStorage()
            .UseAppServiceVirtualNetworking();
});

builder.Services.AddServicesForSelfHostedDashboard();

var app = builder.Build();
app.UseOrleansDashboard();


app.Run();

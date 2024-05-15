var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRockPaperOrleans();
builder.AddApplicationComponents();

var app = builder.Build();

app.MapDefaultEndpoints();
app.AddSwaggerDuringDevelopment();
app.MapGameControllerEndpoints();

app.Run();



using GameController;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRockPaperOrleans();

builder.Services.AddHostedService<GameEngine>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();



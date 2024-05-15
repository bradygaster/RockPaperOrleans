using GameController;
using NSwag;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddApplicationComponents(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<GameEngineStateController>();
        builder.Services.AddHostedService<GameEngine>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApiDocument(options => {
            options.PostProcess = document =>
            {
                document.Info = new OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = "Rock, Paper, Orleans API",
                    Description = "Back-end API for controlling the Rock, Paper, Orleans game server.",
                };
            };
        });

        return builder;
    }
}
public static class WebApplicationExtensions
{
    public static WebApplication MapGameControllerEndpoints(this WebApplication app)
    {
        app.MapGet("start", (GameEngineStateController gameEngineController) => gameEngineController.Start())
           .WithOpenApi(operation =>
           {
               operation.Summary = "Starts the Game Loop";
               operation.OperationId = "startGameLoop";
               return operation;
           });

        app.MapGet("stop", (GameEngineStateController gameEngineController) => gameEngineController.Stop())
           .WithOpenApi(operation =>
           {
               operation.Summary = "Stops the Game Loop";
               operation.OperationId = "stopGameLoop";
               return operation;
           });

        app.MapPost("setGameLoopDelay", (GameEngineStateController gameEngineController, int gameLoopDelay) => gameEngineController.SetGameLoopDelayInterval(gameLoopDelay))
           .WithOpenApi(operation =>
           {
               operation.Summary = "Set Game Loop delay";
               operation.Description = "Sets the number of milliseconds between game loop iterations. A lower number means the games will be played quicker.";
               operation.OperationId = "setGameLoopDelay";
               return operation;
           });

        return app;
    }
    public static WebApplication AddSwaggerDuringDevelopment(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi();
        }

        return app;
    }
}
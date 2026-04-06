using NotesKeeperWebApi.Middleware;
using Serilog;

namespace NotesKeeperWebApi.Configuration;

public static class ConfigureWebApp
{
    public static void AddAppMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        app.UseSerilogRequestLogging();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Notes Keeper API v1");
                options.SwaggerEndpoint("/swagger/v2/swagger.json", "Notes Keeper API v2");
                options.RoutePrefix = string.Empty; // serves the ui at the apps root
            });
        }

        app.Use(async (context, next) =>
        {
            context.Request.EnableBuffering(); // enable buffering to allow reading the request body multiple times in the filters and controllers
            await next();
        });
    }
}
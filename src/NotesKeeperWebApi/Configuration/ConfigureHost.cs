
using Serilog;

namespace NotesKeeperWebApi.Configuration;

public static class ConfigureHost
{    
    public static void AddAndConfigureLoggers(this IHostBuilder host)
    {
        host.UseSerilog((context, services, loggerConfiguration) =>
{
            loggerConfiguration.ReadFrom.Configuration(context.Configuration) // read settings from appsettings.json
                            .ReadFrom.Services(services) // access app services 
                            .Enrich.FromLogContext(); // automatically add contextual info
        });
    }
}
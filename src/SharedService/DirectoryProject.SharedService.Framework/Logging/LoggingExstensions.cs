using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace DirectoryProject.SharedService.Framework.Logging;

public static class LoggingExstensions
{
    public static IServiceCollection AddAPILogging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Debug()
            .WriteTo.Seq(configuration.GetConnectionString("Seq")
                ?? throw new ArgumentNullException("Seq"))
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
            .CreateLogger();

        // to watch serilogs inner errors
        Serilog.Debugging.SelfLog.Enable(Console.Error);

        services.AddSerilog();

        return services;
    }

    public static WebApplication UseAPILogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        return app;
    }
}

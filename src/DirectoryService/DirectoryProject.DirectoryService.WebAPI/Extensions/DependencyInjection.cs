using DirectoryProject.DirectoryService.Infrastructure.Database;
using DirectoryProject.DirectoryService.WebAPI.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace DirectoryProject.DirectoryService.WebAPI.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // disables controller's filter, that validaties model state before entering controller;
        // instead, it will pass invalid model to controller and then to application layer,
        // where we validate the model with fluent validation
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAPILogging(configuration);

        Infrastructure.DependencyInjection.AddInfrastructure(services, configuration);
        Application.DependencyInjection.AddApplication(services, configuration);

        return services;
    }

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

    public static async Task<WebApplication> ConfigureAsync(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            await using var scope = app.Services.CreateAsyncScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            await dbContext.ApplyMigrationsAsync();

            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSerilogRequestLogging();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}

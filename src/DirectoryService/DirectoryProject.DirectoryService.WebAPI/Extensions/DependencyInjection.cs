using DirectoryProject.DirectoryService.Infrastructure.DatabaseWrite;
using Framework.Logging;
using Framework.Middlewares;
using Microsoft.AspNetCore.Mvc;

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

    public static async Task<WebApplication> ConfigureAsync(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            await using var scope = app.Services.CreateAsyncScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationWriteDBContext>();
            await dbContext.ApplyMigrationsAsync();

            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAPILogging();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}

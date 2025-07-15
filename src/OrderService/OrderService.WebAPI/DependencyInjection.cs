using System.Reflection;
using Framework.Endpoints;
using Framework.Logging;
using Framework.Middlewares;
using Microsoft.AspNetCore.Mvc;
using OrderService.WebAPI.Database;

namespace OrderService.WebAPI;

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

        var dbConnectionString = configuration.GetConnectionString(ApplicationDBContext.DATABASE_CONFIGURATION);
        services.AddScoped(_ => new ApplicationDBContext(dbConnectionString));
        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddEndpoints(Assembly.GetExecutingAssembly());
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAPILogging(configuration);

        return services;
    }

    public static async Task<WebApplication> ConfigureAsync(
        this WebApplication app)
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

        app.UseAPILogging();

        app.UseHttpsRedirection();

        app.MapEndpoints();

        return app;
    }
}

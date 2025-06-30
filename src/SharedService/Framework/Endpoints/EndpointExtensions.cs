using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Endpoints;
public static class EndpointsExtensions
{
    public static IApplicationBuilder MapEndpoints(
        this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder = null)
    {
        var endpoints = app.Services.GetServices<IEndpoint>();

        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (var endpoint in endpoints) endpoint.MapEndpoint(builder);

        return app;
    }

    public static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        Assembly assembly)
    {
        var assemblyTypes = assembly.DefinedTypes;

        foreach (Type type in assemblyTypes)
        {
            if (type.IsAbstract || type.IsClass == false)
                continue;

            var endpointInterfaceType = type.GetInterfaces().FirstOrDefault(i =>
                i.Name! == typeof(IEndpoint).Name);
            if (endpointInterfaceType is not null)
            {
                services.AddTransient(endpointInterfaceType, type);
                continue;
            }
        }

        return services;
    }
}
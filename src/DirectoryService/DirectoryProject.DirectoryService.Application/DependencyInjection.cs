using System.Reflection;
using System.Text.Json;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Application.Shared.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryProject.DirectoryService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplicationHandlersWithValidators(
            Assembly.GetAssembly(typeof(DependencyInjection))!,
            configuration);

        services.AddSlugService(configuration);

        return services;
    }

    public static IServiceCollection AddSlugService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var slugSettingsJson = File.ReadAllText("etc/slugSettings.json");

        var settings = JsonSerializer.Deserialize<SlugService.Settings>(slugSettingsJson);
        if (settings is null)
            throw new ApplicationException("Could not deserialize slug settings from slugSettings.json");

        services.AddSingleton(_ => new SlugService(settings));

        return services;
    }

    public static IServiceCollection AddApplicationHandlersWithValidators(
        this IServiceCollection services,
        Assembly assembly,
        IConfiguration configuration)
    {
        var assemblyTypes = GetLoadableTypes(assembly);

        foreach (Type type in assemblyTypes)
        {
            if (type.IsAbstract || type.IsClass == false)
                continue;

            // register fluent validation validators
            if (type.BaseType?.Name == typeof(AbstractValidator<>).Name)
                services.AddScoped(type.BaseType, type);

            // check if type implements ICommandHandler interface
            var interfaceType = type.GetInterfaces().FirstOrDefault(i =>
                i.Name! == typeof(ICommandHandler<>).Name || i.Name! == typeof(ICommandHandler<,>).Name);
            if (interfaceType == null)
                continue;

            services.AddScoped(interfaceType, type);
        }

        return services;
    }

    private static List<Type> GetLoadableTypes(this Assembly assembly)
    {
        try
        {
            return assembly.GetTypes().ToList();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.Where(t => t != null).ToList()!;
        }
    }
}

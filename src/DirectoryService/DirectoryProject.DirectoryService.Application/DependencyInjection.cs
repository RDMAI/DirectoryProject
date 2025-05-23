using System.Reflection;
using System.Text.Json;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain.DepartmentValueObjects;
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
        // Add slug replace dictionary to DepartmentPath value object
        var slugReplaceDictionaryJson = File.ReadAllText("etc/slugReplaceDictionary.json");
        var slugReplaceDictionary = JsonSerializer.Deserialize<Dictionary<char, string>>(slugReplaceDictionaryJson);
        if (slugReplaceDictionary is null)
            throw new ApplicationException("Could not deserialize slug settings from slugSettings.json");
        DepartmentPath.SetReplaceChars(slugReplaceDictionary);

        services.AddApplicationHandlersWithValidators(
            Assembly.GetAssembly(typeof(DependencyInjection))!,
            configuration);

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

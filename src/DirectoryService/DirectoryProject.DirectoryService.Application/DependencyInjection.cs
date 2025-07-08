using System.Reflection;
using System.Text.Json;
using Core;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.FileService.Communication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryProject.DirectoryService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add slug replace dictionary to Department
        var slugReplaceDictionaryJson = File.ReadAllText("etc/slugReplaceDictionary.json");
        var slugReplaceDictionary = JsonSerializer.Deserialize<Dictionary<char, string>>(slugReplaceDictionaryJson);
        if (slugReplaceDictionary is null)
            throw new ApplicationException("Could not deserialize slug settings from slugSettings.json");
        Department.SetReplaceChars(slugReplaceDictionary);

        services.AddApplicationHandlersWithValidators(
            Assembly.GetAssembly(typeof(DependencyInjection))!,
            configuration);

        services.AddFileHttpCommunication(configuration);

        return services;
    }
}

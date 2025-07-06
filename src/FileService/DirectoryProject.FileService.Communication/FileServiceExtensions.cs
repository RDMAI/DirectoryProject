using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DirectoryProject.FileService.Communication;

public static class FileServiceExtensions
{
    public static IServiceCollection AddFileHttpCommunication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var fileSection = configuration.GetSection(FileServiceOptions.SECTION_NAME)
            ?? throw new ApplicationException("S3 is misconfigured");
        services.Configure<FileServiceOptions>(fileSection);

        services.AddHttpClient<IFileService, FileHttpClient>((serviceProvider, config) =>
        {
            var fileOptions = serviceProvider.GetRequiredService<IOptions<FileServiceOptions>>().Value;

            config.BaseAddress = new Uri(fileOptions.BaseURL);
        });

        return services;
    }
}

using System.Reflection;
using Amazon.S3;
using DirectoryProject.FileService.WebAPI.FileManagement;
using Framework.Endpoints;
using Framework.Logging;
using Framework.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryProject.FileService.WebAPI;

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

        services.AddEndpoints(Assembly.GetExecutingAssembly());
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAPILogging(configuration);

        // S3 configuration
        var s3Section = configuration.GetSection(S3Options.S3_SECTION)
            ?? throw new ApplicationException("S3 is misconfigured");
        services.Configure<S3Options>(s3Section);

        var s3Options = s3Section.Get<S3Options>()
            ?? throw new ApplicationException("S3 is misconfigured");

        services.AddSingleton<IS3Provider, S3Provider>();
        services.AddSingleton<IAmazonS3>(_ =>
        {
            var config = new AmazonS3Config
            {
                ServiceURL = s3Options.Endpoint,
                ForcePathStyle = true,
                UseHttp = !s3Options.WithSSL,
            };

            return new AmazonS3Client(s3Options.AccessKey, s3Options.SecretKey, config);
        });

        // cors
        services.AddCors();

        services.AddAntiforgery();

        return services;
    }

    public static async Task<WebApplication> ConfigureAsync(
        this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            await app.CreateRequiredBucketsAsync();

            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAPILogging();

        app.UseHttpsRedirection();

        app.UseCors(builder =>
        {
            builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
        });

        app.UseAntiforgery();

        app.MapEndpoints();

        return app;
    }

    private static async Task CreateRequiredBucketsAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();

        var amazonS3 = scope.ServiceProvider.GetRequiredService<IAmazonS3>();

        foreach (var bucketName in S3Provider.BucketNames)
            await amazonS3.EnsureBucketExistsAsync(bucketName);
    }
}

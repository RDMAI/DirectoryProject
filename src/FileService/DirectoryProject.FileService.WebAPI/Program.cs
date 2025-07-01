using DirectoryProject.FileService.WebAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProgramDependencies(builder.Configuration);

var app = builder.Build();

await app.ConfigureAsync();

app.Run();

public partial class Program;  // to access Program class in test projects
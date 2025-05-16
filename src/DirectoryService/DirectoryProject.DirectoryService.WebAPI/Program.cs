using DirectoryProject.DirectoryService.WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProgramDependencies(builder.Configuration);

var app = builder.Build();

app.Configure();

app.Run();

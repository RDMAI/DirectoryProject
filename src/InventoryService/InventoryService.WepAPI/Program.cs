using InventoryService.WepAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProgramDependencies(builder.Configuration);

var app = builder.Build();

await app.ConfigureAsync();

app.Run();

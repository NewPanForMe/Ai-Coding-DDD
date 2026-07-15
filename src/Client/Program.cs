using Client.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();

var app = builder.Build();

await app.SeedDataAsync();
app.UseApplicationMiddleware();

app.Run();

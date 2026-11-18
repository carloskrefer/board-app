using App.Migrations;
using App.DependencyInjection;
using App.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppServices();

var app = builder.Build();

app.UseAppMiddleware();

await app.ApplyModulesMigrations();

app.Run();

public partial class Program; // Required to run integration tests with WebApplicationFactory<Program>.
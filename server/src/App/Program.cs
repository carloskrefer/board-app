using App.DependencyInjection;
using App.Migrations;
using Board.App.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.AddAuthentication();
builder.AddModules();

builder.Services.AddControllers(options => options.ReturnHttpNotAcceptable = true);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<TraceIdHeaderMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.ApplyModulesMigrations();

app.Run();

// TODO: Capture exceptions to hide internal error details by creating my own exception middleware

// Required to run integration tests with WebApplicationFactory<Program>.
public partial class Program;
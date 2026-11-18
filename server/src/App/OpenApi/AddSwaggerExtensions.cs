using System.Reflection;
using App.OpenApi.DocumentFilters;
using App.OpenApi.OperationFilters;
using Core.Api.OpenApi;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.OpenApi;

internal static class AddSwaggerExtensions
{
    public static void AddSwaggerServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            IncludeXmlComments(options);
            SwaggerDoc(options);
            AddSecurityDefinition(options);
            AddServer(options);
            AddOperationFilters(options);
            AddDocumentFilters(options);
            options.SupportNonNullableReferenceTypes();
        });
    }

    private static void AddOperationFilters(SwaggerGenOptions options)
    {
        options.OperationFilter<ProblemJsonOperationFilter>();
        options.OperationFilter<SetCookieOperationFilter>();
    }

    private static void AddDocumentFilters(SwaggerGenOptions options)
    {
        options.DocumentFilter<TagFilter>();
    }

    private static void SwaggerDoc(SwaggerGenOptions options) =>
        options.SwaggerDoc(Versions.Version1, CreateOpenApiInfo());

    private static void IncludeXmlComments(SwaggerGenOptions options)
    {
        foreach (var xmlFile in GetAllXmlFileNames())
        {
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);
        }
    }

    private static string[] GetAllXmlFileNames() =>
    [
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml",
        "Contracts.xml",
        "Auth.Api.xml"
    ];

    private static OpenApiInfo CreateOpenApiInfo() => new()
    {
        Version = Versions.Version1,
        Title = "Board App Server API",
        Description = "Board App Server API for managing boards, tasks, and users.",
    };

    private static void AddSecurityDefinition(SwaggerGenOptions options) =>
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Please enter a valid JWT Access Token",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
        });

    private static void AddServer(SwaggerGenOptions options) =>
        options.AddServer(new OpenApiServer
        {
            Url = "http://localhost:5047", // TODO: Change whenever it is deployed to production. Check HTTPs later too.
            Description = "Production server"
        });
}
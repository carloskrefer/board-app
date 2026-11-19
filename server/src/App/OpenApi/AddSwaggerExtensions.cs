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
            AddSecurity(options);
            AddServer(options);
            AddOperationFilters(options);
            AddDocumentFilters(options);
            options.SupportNonNullableReferenceTypes();
        });
    }

    private static void AddSecurity(SwaggerGenOptions options)
    {
        AddSecurityDefinition(options);
        AddSecurityRequirement(options);
    }

    private static void AddSecurityRequirement(SwaggerGenOptions options)
    {
        var requirement = CreateOpenApiSecurityRequirement();
        options.AddSecurityRequirement(requirement);
    }

    private static void AddOperationFilters(SwaggerGenOptions options)
    {
        options.OperationFilter<ProblemJsonOperationFilter>();
        options.OperationFilter<SetCookieOperationFilter>();
        options.OperationFilter<OperationSecurityRequirementOperationFilter>();
        options.OperationFilter<AllowAnonymousOperationFilter>();
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

    private static void AddSecurityDefinition(SwaggerGenOptions options)
    {
        AddBearerSecurityDefinition(options);
        AddRefreshTokenSecurityDefinition(options);
    }

    private static OpenApiSecurityRequirement CreateOpenApiSecurityRequirement()
    {
        var requirement = new OpenApiSecurityRequirement();
        requirement[CreateAccessTokenSecuritySchemeReference()] = Array.Empty<string>();
        return requirement;
    }

    private static OpenApiSecurityScheme CreateAccessTokenSecuritySchemeReference() =>
        new()
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = SecuritySchemeNames.AccessToken
            }
        };

    private static void AddBearerSecurityDefinition(SwaggerGenOptions options) =>
        options.AddSecurityDefinition(SecuritySchemeNames.AccessToken, new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Description = "Access Token (JWT)",
            Scheme = "bearer",
            BearerFormat = "JWT",
        });

    private static void AddRefreshTokenSecurityDefinition(SwaggerGenOptions options) =>
        options.AddSecurityDefinition(SecuritySchemeNames.RefreshToken, new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            Description = "Refresh Token (Session Id)",
            Name = "refreshToken",
            In = ParameterLocation.Cookie,
        });

    private static void AddServer(SwaggerGenOptions options) =>
        options.AddServer(new OpenApiServer
        {
            Url = "http://localhost:5047", // TODO: Change whenever it is deployed to production. Check HTTPs later too.
            Description = "Production server"
        });
}
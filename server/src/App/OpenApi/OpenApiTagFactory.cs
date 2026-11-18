using Core.Api.OpenApi;
using Microsoft.OpenApi.Models;

namespace App.OpenApi;

internal static class OpenApiTagFactory
{
    public static OpenApiTag CreateAuthTag() => new()
    {
        Name = Tags.Auth,
        Description = "Operations related to user authentication and authorization.",
    };
}
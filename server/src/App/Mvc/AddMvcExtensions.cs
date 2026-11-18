using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Contracts.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace App.Mvc;

internal static class AddMvcExtensions
{
    public static void AddMvcServices(this WebApplicationBuilder builder) =>
        builder.Services
            .AddControllers(ConfigureMvcOptions)
            .AddJsonOptions(ConfigureJsonOptions)
            .ConfigureApiBehaviorOptions(ConfigureApiBehaviorOptions);

    private static void ConfigureJsonOptions(JsonOptions options) =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

    private static void ConfigureApiBehaviorOptions(ApiBehaviorOptions options) =>
        options.SuppressModelStateInvalidFilter = true;

    private static void ConfigureMvcOptions(MvcOptions options)
    {
        ConfigureOutputFormatters(options);
        AddFilters(options);
    }

    private static void ConfigureOutputFormatters(MvcOptions options)
    {
        RemoveStringOutputFormatter(options);
        RemoveTextJsonOutputFormatter(options);
    }

    private static void RemoveStringOutputFormatter(MvcOptions options) =>
        options.OutputFormatters.RemoveType<StringOutputFormatter>();

    private static void RemoveTextJsonOutputFormatter(MvcOptions options) =>
        options.OutputFormatters
            .OfType<SystemTextJsonOutputFormatter>()
            .FirstOrDefault()
            ?.SupportedMediaTypes.Remove("text/json");

    private static void AddFilters(MvcOptions options)
    {
        options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
        options.Filters.Add(new ProducesResponseTypeAttribute<ApiProblemDetails>(StatusCodes.Status400BadRequest));
        options.Filters.Add(new ProducesResponseTypeAttribute<ApiProblemDetails>(StatusCodes.Status500InternalServerError));
    }
}
using System.Text.RegularExpressions;
using Core.Api.OpenApi.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.OpenApi.OperationFilters;

// TODO: Review this code, even though it works
public sealed class OperationSecurityRequirementOperationFilter : IOperationFilter
{
    // name            -> anything except brackets, commas and whitespace
    // [scopes]        -> optional, anything except brackets
    private static readonly Regex SchemeRegex = new(
        @"^\s*(?<name>[^\[\]\s,]+)\s*(?:\[(?<scopes>[^\[\]]*)\])?\s*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly char[] ScopeSeparators = { ',', ' ', '\t' };

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var methodInfo = context.MethodInfo;

        // 1) Attributes on the method take precedence
        var attributes = methodInfo
            .GetCustomAttributes(typeof(OperationSecurityRequirementAttribute), inherit: true)
            .Cast<OperationSecurityRequirementAttribute>()
            .ToList();

        // 2) Otherwise use the ones on the class (including base classes)
        if (attributes.Count == 0)
        {
            var controllerType = methodInfo.ReflectedType ?? methodInfo.DeclaringType;

            if (controllerType is not null)
            {
                attributes = controllerType
                    .GetCustomAttributes(typeof(OperationSecurityRequirementAttribute), inherit: true)
                    .Cast<OperationSecurityRequirementAttribute>()
                    .ToList();
            }
        }

        // Nothing declared: keep default behavior (global security, if any)
        if (attributes.Count == 0)
            return;

        // Each attribute becomes one OpenApiSecurityRequirement (OR between them);
        // inside each one, all schemes are required together (AND).
        operation.Security = attributes
            .Select(attribute => CreateRequirement(attribute, context))
            .ToList();
    }

    private static OpenApiSecurityRequirement CreateRequirement(
        OperationSecurityRequirementAttribute attribute,
        OperationFilterContext context)
    {
        // Preserves declaration order; if the same scheme is listed twice, scopes are merged.
        var scopesByScheme = new Dictionary<string, List<string>>(StringComparer.Ordinal);

        foreach (var raw in attribute.Schemes)
        {
            var match = SchemeRegex.Match(raw ?? string.Empty);

            if (!match.Success)
            {
                throw new InvalidOperationException(
                    $"Invalid scheme declaration '{raw}' in [SecurityRequirementObject] affecting " +
                    $"{context.MethodInfo.DeclaringType?.Name}.{context.MethodInfo.Name}. " +
                    "Expected 'scheme' or 'scheme[scope1, scope2]'.");
            }

            var name = match.Groups["name"].Value;
            var scopes = match.Groups["scopes"].Value
                .Split(ScopeSeparators, StringSplitOptions.RemoveEmptyEntries);

            if (!scopesByScheme.TryGetValue(name, out var list))
            {
                list = new List<string>();
                scopesByScheme[name] = list;
            }

            foreach (var scope in scopes)
            {
                if (!list.Contains(scope))
                    list.Add(scope);
            }
        }

        var requirement = new OpenApiSecurityRequirement();

        foreach (var (name, scopes) in scopesByScheme)
        {
            var scheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = name // must match the name used in AddSecurityDefinition
                }
            };

            requirement[scheme] = scopes;
        }

        return requirement;
    }
}
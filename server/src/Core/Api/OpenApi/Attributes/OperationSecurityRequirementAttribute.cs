namespace Core.Api.OpenApi.Attributes;

/// <summary>
/// Declares one OpenAPI's Security Requirement Object for an operation.
/// </summary>
/// 
/// <remarks>
/// <para>
/// For for information, check the "security" field in the
/// <see href="https://spec.openapis.org/oas/latest.html#operation-object">
/// Operation Object (Open API Spec, Section 4.10)</see>.
/// </para>
/// 
/// <para>
/// <example>
/// Example of use:
/// 
/// <code>
///     [SecurityRequirementObject("apiKey")]
///     [SecurityRequirementObject("oauth2[read:orders, write:orders]", "tenantKey")]
///     public IActionResult MyEndpoint() { ... }
///     // => (apiKey) OR (oauth2 [read:orders, write:orders] AND tenantKey)
/// </code>
/// 
/// </example>
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class OperationSecurityRequirementAttribute : Attribute
{
    public string[] Schemes { get; }

    /// <inheritdoc cref="OperationSecurityRequirementAttribute"/>
    public OperationSecurityRequirementAttribute(params string[] schemes)
    {
        Schemes = schemes ?? Array.Empty<string>();
    }
}
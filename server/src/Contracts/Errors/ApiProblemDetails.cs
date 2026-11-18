using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Contracts.Errors;

public record ApiError(string Code, string Message);

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorLocation
{
    Body,
    Query,
    Path,
    Header
}

public record DetailedApiError : ApiError
{
    public string? Field { get; }
    public ErrorLocation? Location { get; }
    public string? RejectedValue { get; }

    [JsonConstructor]
    public DetailedApiError(
        string code, 
        string message, 
        string? field = null, 
        ErrorLocation? location = null, 
        string? rejectedValue = null) 
        : base(code, message)
    {
        Field = field;
        Location = location;
        RejectedValue = rejectedValue;
    }

    public DetailedApiError(
        ApiError generalError, 
        string? field = null, 
        ErrorLocation? location = null, 
        string? rejectedValue = null) 
        : base(generalError.Code, generalError.Message)
    {
        Field = field;
        Location = location;
        RejectedValue = rejectedValue;
    }
}

public class ApiProblemDetails: ProblemDetails
{
    public string? TraceId { get; set; }
    
    /// <summary>
    /// List of errors that occurred while processing the request.
    /// </summary>
    /// <remarks>
    /// Read more: ADR-0001 (docs/adr/0001-errors-extension-as-list.md).
    /// </remarks>
    public DetailedApiError[] Errors { get; set; } = [];
}
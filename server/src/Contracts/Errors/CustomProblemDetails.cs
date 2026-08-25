using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Contracts.Errors;

public record BaseError(string Code, string Message);

public enum ErrorLocationEnum
{
    Body = 0,
    Query = 1,
    Path = 2,
    Header = 3
}

public record DetailedError : BaseError
{
    public string? Field { get; }
    public ErrorLocationEnum? Location { get; }
    public string? RejectedValue { get; }

    [JsonConstructor]
    public DetailedError(
        string code, 
        string message, 
        string? field = null, 
        ErrorLocationEnum? location = null, 
        string? rejectedValue = null) 
        : base(code, message)
    {
        Field = field;
        Location = location;
        RejectedValue = rejectedValue;
    }

    public DetailedError(
        BaseError generalError, 
        string? field = null, 
        ErrorLocationEnum? location = null, 
        string? rejectedValue = null) 
        : base(generalError.Code, generalError.Message)
    {
        Field = field;
        Location = location;
        RejectedValue = rejectedValue;
    }
}

public class CustomProblemDetails: ProblemDetails
{
    public string? TraceId { get; set; }
    public DetailedError[] Errors { get; set; } = [];
}
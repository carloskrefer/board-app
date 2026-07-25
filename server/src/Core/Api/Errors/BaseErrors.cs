using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Errors;

public record GeneralResponseError(string Code, string Message);

public enum ErrorLocationEnum
{
    Body = 0,
    Query = 1,
    Path = 2,
    Header = 3
}

public record DetailedResponseError : GeneralResponseError
{
    public string? Field { get; }
    public ErrorLocationEnum? Location { get; }
    public string? RejectedValue { get; }

    [JsonConstructor]
    public DetailedResponseError(
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

    public DetailedResponseError(
        GeneralResponseError generalError, 
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

public class ApiProblemDetails: ProblemDetails
{
    public string? TraceId { get; set; }
    public DetailedResponseError[] Errors { get; set; } = [];
}
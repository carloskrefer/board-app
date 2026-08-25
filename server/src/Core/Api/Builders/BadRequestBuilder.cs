using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Contracts.Errors;

namespace Core.Api.Builders;

public static class BadRequestConstants
{
    public const string TypeDefault = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
    public const string TitleGeneral = "Bad Request";
    public const string TitleDetailed = "One or more validation errors occurred.";
    public const string DetailDetailed = "See the errors property for details.";
}

public class BadRequestBuilder
{
    private readonly List<BaseError> _errors = new List<BaseError>();
    private readonly ControllerBase _controller;

    public BadRequestBuilder(ControllerBase controller)
    {
        _controller = controller;
    }

    public BadRequestBuilder AddError(BaseError error)
    {
        _errors.Add(error);
        return this;
    }

    public IActionResult Build()
    {
        var body = new CustomProblemDetails
        {
            Type = BadRequestConstants.TypeDefault,
            Title = BadRequestConstants.TitleGeneral,
            Status = StatusCodes.Status400BadRequest,
            Instance = _controller.HttpContext.Request.Path,
            TraceId = Activity.Current?.TraceId.ToString()
        };

        if (!_errors.Any())
            return _controller.BadRequest(body);

        if (HasSingleErrorAndIsNotDetailed())
        {
            body.Title = _errors[0].Code;
            body.Detail = _errors[0].Message;
        }
        else
        {
            body.Title = BadRequestConstants.TitleDetailed;
            body.Detail = BadRequestConstants.DetailDetailed;
            body.Errors = _errors.Select(e => e as DetailedError ?? new DetailedError(e)).ToArray();
        }

        return _controller.BadRequest(body);
    }

    private bool HasSingleErrorAndIsNotDetailed() => 
        (_errors.Count == 1) && ((_errors[0] as DetailedError) == null);
}
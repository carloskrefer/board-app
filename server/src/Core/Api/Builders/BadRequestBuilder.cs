using System.Diagnostics;
using Core.Api.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Builders;

public static class BadRequestConstants
{
    public const string DefaultTitle = "One or more validation errors occurred.";
    public const string DefaultDetail = "See the errors property for details.";
}

public class BadRequestBuilder
{
    private readonly List<GeneralResponseError> _errors = new List<GeneralResponseError>();
    private readonly ControllerBase _controller;

    public BadRequestBuilder(ControllerBase controller)
    {
        _controller = controller;
    }

    public BadRequestBuilder AddError(GeneralResponseError error)
    {
        _errors.Add(error);
        return this;
    }

    public IActionResult Build()
    {
        if (!_errors.Any())
            return _controller.BadRequest();

        string title;
        string detail;
        DetailedResponseError[] errors = [];

        if (HasSingleErrorAndIsNotDetailed())
        {
            title = _errors[0].Code;
            detail = _errors[0].Message;
        }
        else
        {
            title = BadRequestConstants.DefaultTitle;
            detail = BadRequestConstants.DefaultDetail;
            errors = _errors.Select(e => e as DetailedResponseError ?? new DetailedResponseError(e)).ToArray();
        }

        var response = new ApiProblemDetails
        {
            Title = title,
            Status = StatusCodes.Status400BadRequest,
            Detail = detail,
            Instance = _controller.HttpContext.Request.Path,
            Errors = errors,
            TraceId = Activity.Current?.TraceId.ToString()
        };

        return _controller.BadRequest(response);
    }

    private bool HasSingleErrorAndIsNotDetailed() => 
        (_errors.Count == 1) && ((_errors[0] as DetailedResponseError) == null);
}
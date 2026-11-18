using Contracts.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Core.Api;

// TODO: Rename, remove 2
public sealed class ProblemResultFactory2(
    ProblemDetailsFactory factory, 
    IHttpContextAccessor accessor)
{
    private readonly HttpContext context = accessor.HttpContext 
        ?? throw new InvalidOperationException("HttpContext is null");

    // TODO: Use standardized type, title
    public ObjectResult CreateUpdateConflict(string resourceName, string? resourceId = null)
    {
        var detail = ConcurrencyHelper.GenerateUpdateConflictDetail(resourceName, resourceId);

        var problemDetails = factory.CreateProblemDetails(
            httpContext: context,
            statusCode: StatusCodes.Status409Conflict,
            detail: detail);

        return new ObjectResult(problemDetails);
    }

    public ObjectResult CreateBadRequest(IReadOnlyList<DetailedApiError> errors)
    {
        var problemDetails = factory.CreateProblemDetails(
            httpContext: context,
            type: BadRequestConstants.Type,
            title: BadRequestConstants.Title,
            detail: BadRequestConstants.Title,
            statusCode: StatusCodes.Status400BadRequest);

        if (!errors.Any())
            return new ObjectResult(problemDetails);

        problemDetails.Title = BadRequestConstants.Title;
        problemDetails.Detail = BadRequestConstants.DetailForErrorsExtension;
        problemDetails.Extensions["errors"] = errors;

        return new ObjectResult(problemDetails);
    }
}
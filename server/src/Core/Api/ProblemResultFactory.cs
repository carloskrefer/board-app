using System.Net;
using Contracts.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Core.Api;

// TODO: See if I can inject this (if the dependencies bellow are injectable)
public sealed class ProblemResultFactory(
    ProblemDetailsFactory factory, 
    HttpContext context)
{
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

    // TODO: Maybe always use title with the name of the status, and use the detail for the message
    // TODO: I think I should add to the errors array even when there is no specific field, because i have the code,
    // and it is weird to add this code in the title, I think it should be in the errors array.
    // TODO: Acho que remove esse qd ng mais usar, porque o de baixo agora faz da maneira correta.
    public ObjectResult CreateBadRequest(List<ApiError> errors)
    {
        var problemDetails = factory.CreateProblemDetails(
            httpContext: context,
            type: BadRequestConstants.Type,
            title: BadRequestConstants.Title,
            statusCode: StatusCodes.Status400BadRequest);

        if (!errors.Any())
            return new ObjectResult(problemDetails);

        if (errors.HasSingleErrorAndIsNotDetailed())
        {
            problemDetails.Title = errors.First().Code;
            problemDetails.Detail = errors.First().Message;
        }
        else
        {
            problemDetails.Title = BadRequestConstants.Title;
            problemDetails.Detail = BadRequestConstants.DetailForErrorsExtension;
            // TODO: Do something about this extension, maybe should be set in the ProblemResult? Like 'SetErrors'.
            // Because hard coded string wont cut. Ah, a PROBLEM DETAILS BUILDER will do!
            problemDetails.Extensions["errors"] = errors.Select(e => e as DetailedApiError ?? new DetailedApiError(e)).ToArray();
        }

        return new ObjectResult(problemDetails);
    }

    public ObjectResult CreateBadRequest2(IReadOnlyList<DetailedApiError> errors)
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
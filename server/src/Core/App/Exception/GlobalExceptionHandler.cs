using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Board.App.Exception;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        System.Exception exception,
        CancellationToken cancellationToken)
    {
        const int status = StatusCodes.Status500InternalServerError;
        context.Response.StatusCode = status;

        // TODO: Use the object result instead of directly writing in the response body, so it respects the 
        // AddControllers configurations (like content negotiation).
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = new ProblemDetails { Status = status }
        });
    }
}
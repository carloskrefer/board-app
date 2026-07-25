using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Board.App.Middleware;

public static class TraceIdConstants
{
    public const string HeaderName = "X-Trace-Id";
}

public sealed class TraceIdHeaderMiddleware
{
    private readonly RequestDelegate _next;

    public TraceIdHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            if (Activity.Current?.TraceId is not null)
                context.Response.Headers[TraceIdConstants.HeaderName] = Activity.Current.TraceId.ToString();

            return Task.CompletedTask;
        });

        await _next(context);
    }
}
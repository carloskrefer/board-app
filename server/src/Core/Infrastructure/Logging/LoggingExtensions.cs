using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Logging;

public static class ILoggerExtensions
{
    public static void LogInvalidData(this ILogger logger, Error error, object invalidData)
    {
        logger.LogWarning(
            "[{Code}] {Description} Invalid data: {@InvalidData}. TraceId: {TraceId}.",
            error.Code,
            error.Description,
            invalidData,
            Activity.Current?.TraceId.ToString());
    }

    public static void LogStartingOperation(this ILogger logger, string operationName, object data)
    {
        logger.LogInformation(
            "Operation {OperationName} started with data: {@Data}. TraceId: {TraceId}.",
            operationName.ToLowerInvariant(),
            data,
            Activity.Current?.TraceId.ToString());
    }

    public static IHandler<TInput, Result<TOutput>> DecorateWithLogger<THandler, TInput, TOutput>(
        this IHandler<TInput, Result<TOutput>> handler,
        IServiceProvider provider,
        Projections<TInput, TOutput> projections,
        string handlerName
    )
        where THandler : IHandler<TInput, Result<TOutput>>
    {
        var logger =
            provider.GetRequiredService<
                ILogger<LoggingDecorator<TInput, Result<TOutput>, TOutput>>
            >();

        return new LoggingDecorator<TInput, Result<TOutput>, TOutput>(
            logger,
            handler,
            handlerName,
            projections);
    }
}
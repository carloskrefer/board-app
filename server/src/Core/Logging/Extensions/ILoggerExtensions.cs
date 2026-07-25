using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Core.Logging.Extensions;

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
}
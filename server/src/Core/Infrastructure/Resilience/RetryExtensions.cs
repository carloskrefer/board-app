using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace Core.Infrastructure.Resilience;

public static class ILoggerExtensions
{
    public static IHandler<TInput, Result<TOutput>> DecorateWithRetrier<THandler, TInput, TOutput>(
        this IHandler<TInput, Result<TOutput>> handler,
        IServiceProvider provider,
        Func<Exception, bool> isRetryable,
        string handlerName,
        int maxAttempts = 3
    )
        where THandler : IHandler<TInput, Result<TOutput>>
    {
        var logger =
            provider.GetRequiredService<
                ILogger<RetryDecorator<TInput, Result<TOutput>, TOutput>>
            >();

        return new RetryDecorator<TInput, Result<TOutput>, TOutput>(
            logger,
            handler,
            handlerName,
            isRetryable,
            maxAttempts);
    }
}
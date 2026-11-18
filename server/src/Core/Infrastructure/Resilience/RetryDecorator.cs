using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Resilience;

public class RetryDecorator<TInput, TOutput, TOutputValue> : IHandler<TInput, TOutput>
    where TOutput : Result<TOutputValue>
{
    private const int MaxAttempts = 3;
    private static readonly TimeSpan BaseDelay = TimeSpan.FromMilliseconds(100);

    private readonly IHandler<TInput, TOutput> _handler;
    private readonly ILogger<RetryDecorator<TInput, TOutput, TOutputValue>> _logger;
    private readonly string _handlerName;
    private readonly Func<Exception, bool> _isRetryable;
    private readonly Random _random = Random.Shared;

    public RetryDecorator(
        ILogger<RetryDecorator<TInput, TOutput, TOutputValue>> logger,
        IHandler<TInput, TOutput> handler,
        string handlerName,
        Func<Exception, bool> isRetryable)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _handlerName = handlerName ?? throw new ArgumentNullException(nameof(handlerName));
        _isRetryable = isRetryable ?? throw new ArgumentNullException(nameof(isRetryable));
    }

    public async Task<TOutput> HandleAsync(TInput input, CancellationToken ct)
    {
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                return await _handler.HandleAsync(input, ct);
            }
            catch (Exception ex) when (attempt < MaxAttempts && _isRetryable(ex))
            {
                var delay = ComputeBackoffWithJitter(attempt);

                _logger.LogWarning(
                    ex,
                    "Handler {Name} failed on attempt {Attempt}/{MaxAttempts}. Retrying in {Delay}ms.",
                    _handlerName,
                    attempt,
                    MaxAttempts,
                    delay.TotalMilliseconds);

                await Task.Delay(delay, ct);
            }
        }

        return await _handler.HandleAsync(input, ct);
    }

    private TimeSpan ComputeBackoffWithJitter(int attempt)
    {
        var exponentialBackoff = BaseDelay * Math.Pow(2, attempt - 1);
        var jitteredMs = _random.NextDouble() * exponentialBackoff.TotalMilliseconds;

        return TimeSpan.FromMilliseconds(jitteredMs);
    }
}
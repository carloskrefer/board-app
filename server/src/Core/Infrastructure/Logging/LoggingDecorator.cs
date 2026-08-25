using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Logging;

public class Projections<TInput, TOutputValue>
{
    private static readonly string NO_PROJECTION = "No defined projection";
    public Func<TInput, object> Input { get; set; } = input => NO_PROJECTION;
    public Func<TOutputValue, object> Output { get; set; } = input => NO_PROJECTION;
}

public class LoggingDecorator<TInput, TOutput, TOutputValue> : IHandler<TInput, TOutput>
    where TOutput : Result<TOutputValue>
{
    private IHandler<TInput, TOutput> _handler;
    private ILogger<LoggingDecorator<TInput, TOutput, TOutputValue>> _logger;
    private readonly string _handlerName;
    private Projections<TInput, TOutputValue> _projections;

    public LoggingDecorator(
        ILogger<LoggingDecorator<TInput, TOutput, TOutputValue>> logger, 
        IHandler<TInput, TOutput> handler,
        string handlerName,
        Projections<TInput, TOutputValue> projections) 
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _handlerName = handlerName ?? throw new ArgumentNullException(nameof(handlerName));
        _projections = projections ?? throw new ArgumentNullException(nameof(projections));
    }
    
    public async Task<TOutput> HandleAsync(TInput input, CancellationToken ct)
    {
        LogBefore(input);

        var result = await _handler.HandleAsync(input, ct);

        LogAfter(input, result);

        return result;
    }

    private void LogBefore(TInput input)
    {
        _logger.LogInformation(
            "Handler {UseCaseName} started. Input: {@Input}. TraceId: {TraceId}.", 
            _handlerName, 
            _projections.Input(input), 
            Activity.Current?.TraceId.ToString()); 
    }

    private void LogAfter(TInput input, Result<TOutputValue> result)
    {
        if (result.IsFailure)
            _logger.LogInvalidData(
                result.Error, 
                _projections.Input(input));
        else
            _logger.LogInformation(
                "Handler {UseCaseName} finished successfully. Output: {@Output}. TraceId: {TraceId}.", 
                _handlerName, 
                _projections.Output(result.Value),
                Activity.Current?.TraceId.ToString());
    }
}

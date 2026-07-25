using System.Diagnostics;
using Core.Logging.Extensions;
using Microsoft.Extensions.Logging;

namespace Core.Logging.Decorators;

public interface Loggable {
    public const string Empty = "Empty";
    public object ToLog();
    public object ToLogIdentity();
}

public class LoggingDecorator<TInput, TOutput, TOutputValue> : IUseCase<TInput, TOutput> 
    where TInput : Loggable
    where TOutput : Result<TOutputValue>
    where TOutputValue : Loggable
{
    private IUseCase<TInput, TOutput> _useCase;
    private ILogger<LoggingDecorator<TInput, TOutput, TOutputValue>> _logger;
    private readonly string useCaseName;

    public LoggingDecorator(
        ILogger<LoggingDecorator<TInput, TOutput, TOutputValue>> logger, 
        IUseCase<TInput, TOutput> useCase,
        string useCaseName) 
    { 
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _useCase = useCase ?? throw new ArgumentNullException(nameof(useCase));
        this.useCaseName = useCaseName ?? throw new ArgumentNullException(nameof(useCaseName));
    }
    
    public async Task<TOutput> ExecuteAsync(TInput input, CancellationToken ct)
    {
        LogBefore(input);

        var result = await _useCase.ExecuteAsync(input, ct);

        LogAfter(input, result);

        return result;
    }

    private void LogBefore(TInput input)
    {
        _logger.LogInformation(
            "Use case {UseCaseName} started. Input identifier: {@Input}. TraceId: {TraceId}.", 
            useCaseName, 
            input.ToLogIdentity(), 
            Activity.Current?.TraceId.ToString()); 
    }

    private void LogAfter(TInput input, Result<TOutputValue> result)
    {
        if (result.IsFailure)
            _logger.LogInvalidData(
                result.Error, 
                input.ToLog());
        else
            _logger.LogInformation(
                "Use case {UseCaseName} finished successfully. Output: {@Output}. TraceId: {TraceId}.", 
                useCaseName, 
                result.Value.ToLog(),
                Activity.Current?.TraceId.ToString());
    }
}

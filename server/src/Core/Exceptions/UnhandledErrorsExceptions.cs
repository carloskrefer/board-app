namespace Core.Exceptions;

public class UnhandledApplicationErrorException : Exception
{
    public UnhandledApplicationErrorException(Error error)
        : base($"An application error was not handled: [{error.Code}] {error.Description}")
    {
    }
}
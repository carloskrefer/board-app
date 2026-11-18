namespace Contracts.Errors;

public static class NullResponseErrors
{
    public static ApiError NullValue =>
        new(
            $"VALUE_IS_NULL",
            $"The field value cannot be null.");
}
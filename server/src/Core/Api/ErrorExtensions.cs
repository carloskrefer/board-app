using Contracts.Errors;

namespace Core.Api;

public static class ErrorExtensions
{
    public static bool HasSingleErrorAndIsNotDetailed(this IEnumerable<ApiError> errors) =>
        (errors.Count() == 1) && ((errors.First() as DetailedApiError) == null);
}
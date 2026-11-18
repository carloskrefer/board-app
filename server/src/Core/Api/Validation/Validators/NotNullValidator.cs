using Contracts.Errors;

namespace Core.Api.Validation.Validators;

public class NotNullValidator(object? value) : IValidator
{
    public IEnumerable<DetailedApiError> GetErrors(ValidationContext context) =>
        value is null  
            ? [context.CreateError(NullResponseErrors.NullValue, null)]
            : [];
}
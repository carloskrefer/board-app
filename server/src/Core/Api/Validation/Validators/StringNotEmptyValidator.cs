using Contracts.Errors;

namespace Core.Api.Validation.Validators;

public class StringNotEmptyValidator(string? text, string subject) : IValidator
{
    public IEnumerable<DetailedApiError> GetErrors(ValidationContext context)
    {
        if (text is null)
            yield break;

        if (text.Equals(string.Empty))
            yield return context.CreateError(TextResponseErrors.Empty(subject), text);
    }
}
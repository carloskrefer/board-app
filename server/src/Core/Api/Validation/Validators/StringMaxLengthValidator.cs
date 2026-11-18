using Contracts.Errors;

namespace Core.Api.Validation.Validators;

public class StringMaxLengthValidator(string text, int maxLength, string subject) : IValidator
{
    public IEnumerable<DetailedApiError> GetErrors(ValidationContext context)
    {
        if (text.Length > maxLength)
            yield return context.CreateError(TextResponseErrors.TooLong(maxLength, subject), text);
    }
}
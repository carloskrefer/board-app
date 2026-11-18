using Contracts.Errors;

namespace Core.Api.Validation.Validators;

public class StringMinLengthValidator(string text, int minLength, string subject) : IValidator
{
    public IEnumerable<DetailedApiError> GetErrors(ValidationContext context)
    {
        if (text.Length < minLength)
            yield return context.CreateError(TextResponseErrors.TooSmall(minLength, subject), text);
    }
}
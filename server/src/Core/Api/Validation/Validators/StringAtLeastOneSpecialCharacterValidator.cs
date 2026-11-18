using Contracts.Errors;
using Core.String.Extensions;

namespace Core.Api.Validation.Validators;

public class StringAtLeastOneSpecialCharacterValidator(string text, string subject) : IValidator
{
    public IEnumerable<DetailedApiError> GetErrors(ValidationContext context)
    {
        if (!text.HasAtLeastOneSpecialCharacter())
            yield return context.CreateError(TextResponseErrors.AtLeastOneSpecialCharacter(subject), text);
    }
}
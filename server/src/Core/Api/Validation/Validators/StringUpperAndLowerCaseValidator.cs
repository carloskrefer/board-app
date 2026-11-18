using Contracts.Errors;
using Core.String.Extensions;

namespace Core.Api.Validation.Validators;

public class StringUpperAndLowerCaseValidator(string text, string subject) : IValidator
{
    public IEnumerable<DetailedApiError> GetErrors(ValidationContext context)
    {
        if (!text.HasAtLeastOneUpperAndLowerCaseLetter())
            yield return context.CreateError(TextResponseErrors.AtLeastOneUpperAndLowerCase(subject), text);
    }
}
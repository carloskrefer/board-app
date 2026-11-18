using Contracts.Errors;
using Core.String.Extensions;

namespace Core.Api.Validation.Validators;

public class StringAtLeastOneNumberValidator(string text, string subject) : IValidator
{
    public IEnumerable<DetailedApiError> GetErrors(ValidationContext context)
    {
        if (!text.HasAtLeastOneNumber())
            yield return context.CreateError(TextResponseErrors.AtLeastOneNumber(subject), text);
    }
}
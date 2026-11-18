using Contracts.Errors;
using Core.String.Extensions;

namespace Core.Api.Validation.Validators;

public class StringNotOnlySpecialCharactersValidator(string text, string subject) : IValidator
{
    public IEnumerable<DetailedApiError> GetErrors(ValidationContext context)
    {
        if (string.IsNullOrEmpty(text))
            yield break;

        if (!text.HasAtLeastOneLetterOrNumber())
            yield return context.CreateError(TextResponseErrors.OnlySpecialCharacters(subject), text);
    }
}
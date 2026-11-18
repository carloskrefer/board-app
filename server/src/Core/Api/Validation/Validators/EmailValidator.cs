using Contracts.Errors;
using Core.Predicates;

namespace Core.Api.Validation.Validators;

public class EmailValidator(string? email) : IValidator
{
    public IEnumerable<DetailedApiError> GetErrors(ValidationContext context)
    {
        if (email is null)
            yield break;

        if (!email.IsEmailFormatValid())
            yield return context.CreateError(EmailResponseErrors.Format, email);
    }
}
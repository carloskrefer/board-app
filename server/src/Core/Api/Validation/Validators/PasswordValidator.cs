using Contracts.Errors;
using Core.Predicates;

namespace Core.Api.Validation.Validators;

public class PasswordValidator(string? password) : CompositeValidator
{
    protected override IEnumerable<IValidator> GetValidators()
    {
        if (password is null)
            return [];

        return [
            new StringMinLengthValidator(password, 6, "Password").WithHiddenValue(true),
            new StringMaxLengthValidator(password, 100, "Password").WithHiddenValue(true),
            new StringAtLeastOneSpecialCharacterValidator(password, "Password").WithHiddenValue(true),
            new StringAtLeastOneNumberValidator(password, "Password").WithHiddenValue(true),
            new StringUpperAndLowerCaseValidator(password, "Password").WithHiddenValue(true)
        ];
    }
}
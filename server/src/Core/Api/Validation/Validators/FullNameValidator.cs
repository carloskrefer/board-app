namespace Core.Api.Validation.Validators;

public class FullNameValidator(string? username) : CompositeValidator
{
    protected override IEnumerable<IValidator> GetValidators()
    {
        if (username is null)
            return [];

        return [
            new StringNotEmptyValidator(username, "Full name"),
            new StringMaxLengthValidator(username, 100, "Full name"),
            new StringNotOnlySpecialCharactersValidator(username, "Full name"),
        ];
    }
}
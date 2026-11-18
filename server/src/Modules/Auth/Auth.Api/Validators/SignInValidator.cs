using Core.Api.Validation;
using Contracts.Auth;
using Core.Api.Validation.Validators;
using Contracts.Errors;
using System.Diagnostics.CodeAnalysis;

namespace Auth.Api.Validators;

public class SignInValidator(SignInRequest? request) : CompositeValidator
{
    private readonly IValidator emailNotNull = new NotNullValidator(request?.Email).ForField(nameof(request.Email));
    private readonly IValidator email = new EmailValidator(request?.Email).ForField(nameof(request.Email));
    private readonly IValidator nameNotNull = new NotNullValidator(request?.Name).ForField(nameof(request.Name));
    private readonly IValidator name = new FullNameValidator(request?.Name).ForField(nameof(request.Name));
    private readonly IValidator passwordNotNull = new NotNullValidator(request?.Password).ForField(nameof(request.Password));
    private readonly IValidator password = new PasswordValidator(request?.Password).ForField(nameof(request.Password));

    protected override IEnumerable<IValidator> GetValidators() => 
    [
        emailNotNull.OnSuccess(email),
        nameNotNull.OnSuccess(name),
        passwordNotNull.OnSuccess(password),
    ];
}

public static class SignInValidatorExtensions
{
    public static bool Validate(
        [NotNullWhen(true)] this SignInRequest? request, 
        out IReadOnlyList<DetailedApiError> errors)
    {
        return new SignInValidator(request).Validate(out errors);
    }
}
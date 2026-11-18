using Core.Api.Validation;
using Contracts.Auth;
using Core.Api.Validation.Validators;
using Contracts.Errors;
using System.Diagnostics.CodeAnalysis;

namespace Auth.Api.Validators;

public class LogInValidator(LogInRequest? request) : CompositeValidator
{
    private readonly IValidator emailNotNull = 
        new NotNullValidator(request?.Email).ForField(nameof(request.Email));
    private readonly IValidator email = 
        new EmailValidator(request?.Email).ForField(nameof(request.Email));
    private readonly IValidator passwordNotNull = 
        new NotNullValidator(request?.Password).ForField(nameof(request.Password));
    private readonly IValidator passwordNotEmpty = 
        new StringNotEmptyValidator(request?.Password, nameof(request.Password))
            .ForField(nameof(request.Password))
            .WithHiddenValue(true);
    
    protected override IEnumerable<IValidator> GetValidators() =>
    [
        emailNotNull.OnSuccess(email),
        passwordNotNull.OnSuccess(passwordNotEmpty)
    ];
}

public static class LogInValidatorExtensions
{
    public static bool Validate(
        [NotNullWhen(true)] this LogInRequest? request, 
        out IReadOnlyList<DetailedApiError> errors)
    {
        return new LogInValidator(request).Validate(out errors);
    }
}
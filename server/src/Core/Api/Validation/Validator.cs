using Contracts.Errors;
using Core.String.Extensions;

namespace Core.Api.Validation;

public sealed record ValidationContext(
    string Path = "", 
    ErrorLocation Location = ErrorLocation.Body,
    bool IsValueHidden = false)
{
    public ValidationContext ForField(string fieldName) =>
        this with { Path = Path.Length == 0 ? fieldName : $"{Path}.{fieldName}" };

    public DetailedApiError CreateError(ApiError error, string? value)
    {
        var valueShown = IsValueHidden ? null : value;
        return new(error, Path, Location, valueShown);
    }
        
}

public interface IValidator
{
    IEnumerable<DetailedApiError> GetErrors(ValidationContext context);
}

public abstract class CompositeValidator : IValidator
{
    protected abstract IEnumerable<IValidator> GetValidators();

    public IEnumerable<DetailedApiError> GetErrors(ValidationContext context) =>
        GetValidators().SelectMany(validator => validator.GetErrors(context)).Distinct();
}

internal sealed class FieldValidator : IValidator
{
    private readonly string fieldName;
    private readonly IValidator inner;

    public FieldValidator(string fieldName, IValidator inner)
    {
        this.fieldName = fieldName.FromPascalCaseToCamelCase();
        this.inner = inner;
    }

    public IEnumerable<DetailedApiError> GetErrors(ValidationContext context) =>
        inner.GetErrors(context.ForField(fieldName));
}

internal sealed class LocationValidator(ErrorLocation location, IValidator inner) : IValidator
{
    public IEnumerable<DetailedApiError> GetErrors(ValidationContext context) =>
        inner.GetErrors(context with { Location = location });
}

internal sealed class HiddenValueValidator(bool isValueHidden, IValidator inner) : IValidator
{
    public IEnumerable<DetailedApiError> GetErrors(ValidationContext context) =>
        inner.GetErrors(context with { IsValueHidden = isValueHidden });
}

internal sealed class OnSuccessValidator(
    IEnumerable<IValidator> validators,
    IEnumerable<IValidator> validatorsOnSuccess) : IValidator
{
    public IEnumerable<DetailedApiError> GetErrors(ValidationContext context)
    {
        var errors = validators.SelectMany(v => v.GetErrors(context)).Distinct();
        return errors.Any() ? errors : validatorsOnSuccess.SelectMany(v => v.GetErrors(context)).Distinct();
    }
}

public static class ValidatorExtensions
{
    public static IValidator ForField(this IValidator validator, string fieldName) =>
        new FieldValidator(fieldName, validator);

    public static IValidator WithLocation(this IValidator validator, ErrorLocation location) =>
        new LocationValidator(location, validator);

    public static IValidator WithHiddenValue(this IValidator validator, bool isValueHidden) =>
        new HiddenValueValidator(isValueHidden, validator);

    public static bool Validate(this IValidator validator, out IReadOnlyList<DetailedApiError> errors)
    {
        errors = validator.GetErrors(new ValidationContext()).ToList();
        return errors.Count == 0;
    }

    public static IValidator OnSuccess(this IEnumerable<IValidator> validators, IEnumerable<IValidator> validatorsOnSuccess) =>
        new OnSuccessValidator(validators, validatorsOnSuccess);
    public static IValidator OnSuccess(this IValidator validator, IEnumerable<IValidator> validatorsOnSuccess) =>
        new OnSuccessValidator([validator], validatorsOnSuccess);
    public static IValidator OnSuccess(this IValidator validator, IValidator validatorOnSuccess) =>
        new OnSuccessValidator([validator], [validatorOnSuccess]);
    public static IValidator OnSuccess(this IEnumerable<IValidator> validators, IValidator validatorsOnSuccess) =>
        new OnSuccessValidator(validators, [validatorsOnSuccess]);
}
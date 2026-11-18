using Auth.Application.Commands;
using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Core.Api;
using Contracts.Auth;
using Microsoft.AspNetCore.Mvc;
using Contracts.Errors;

namespace Auth.Api.Mappers.Extensions;

public static class LogInMappers
{
    public static LogInCommand ToLogInCommand(this LogInRequest request, string? ipAddress, string? userAgent) => 
        new (request.Email, request.Password, ipAddress ?? "Unknown", userAgent ?? "Unknown");

    public static LogInResponse ToLogInResponse(this LogInOutput output) => new (output.CredentialsSerialized);

    public static IActionResult ToResponse(
        this Error error,
        LogInRequest request,
        ProblemResultFactory2 problemResultFactory)
    {
        // TODO: Tem testes pra isso? E pros outros endpoints? Senão acho que deveria ter.
        if (error == OptimisticUnitOfWorkErrors.UpdateConcurrency)
            return problemResultFactory.CreateUpdateConflict(resourceName: "User");

        DetailedApiError? detailedError = null;

        if (error == UserErrors.EmailAlreadyExists)
            detailedError = SignInResponseErrors.EmailAlreadyRegistered(request.Email);

        if (IsCredentialsError(error))
            detailedError = LogInResponseErrors.Credentials;

        List<DetailedApiError> errors = detailedError is null ? [] : [detailedError];

        return problemResultFactory.CreateBadRequest(errors);
    }

    private static bool IsCredentialsError(Error error) =>
        error.HasSameCodeId(PasswordErrors.Id) 
            || error == UserErrors.EmailNotFound 
            || error == UserErrors.IncorrectPassword;
}
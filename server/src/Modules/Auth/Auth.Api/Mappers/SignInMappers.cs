using Auth.Application.Commands;
using Auth.Domain.Entities;
using Contracts.Auth;
using Contracts.Errors;
using Core.Api;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Mappers.Extensions;

public static class SignInMappers
{
    public static SignInCommand ToSignInCommand(this SignInRequest request)
    {
        return new SignInCommand(request.Email, request.Name, request.Password);
    }

    public static SignInResponse ToSignInResponse(this SignInOutput output) => 
        new SignInResponse { UserId = output.UserId };

    public static IActionResult ToResponse(
        this Error error, 
        SignInRequest request,
        ProblemResultFactory2 problemResultFactory)
    {
        DetailedApiError? detailedError = null;

        if (error == UserErrors.EmailAlreadyExists)
            detailedError = SignInResponseErrors.EmailAlreadyRegistered(request.Email);

        List<DetailedApiError> errors = detailedError is null ? [] : [detailedError];

        return problemResultFactory.CreateBadRequest(errors);
    }
}
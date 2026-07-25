using Auth.Api.Controllers;
using Auth.Application.UseCases;
using Auth.Domain.ValueObjects;
using Core.Api.Builders;
using Core.Api.DTOs.Auth.User;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Mappers.Extensions;

public static class SignInRequestExtensions
{
    public static SignInInput ToSignInInput(this SignInRequest request)
    {
        return new SignInInput(request.Email, request.Name, request.Password);
    }

    public static SignInResponse ToSignInResponse(this SignInOutput output) => new (output.UserId);

    public static IActionResult ToBadRequest(
        this SignInRequest request, 
        UserController controller, 
        Error error)
    {
        var builder = new BadRequestBuilder(controller);

        if (error == EmailErrors.Format)
            builder.AddError(SignInResponseErrors.EmailFormat(request.Email));
        if (error == NameErrors.Empty)
            builder.AddError(SignInResponseErrors.NameEmpty(request.Name));
        if (error == NameErrors.TooLong)
            builder.AddError(SignInResponseErrors.NameTooLong(request.Name));
        if (error == NameErrors.OnlySpecialCharacters)
            builder.AddError(SignInResponseErrors.NameOnlySpecialCharacters(request.Name));
        if (error == PasswordErrors.Length)
            builder.AddError(SignInResponseErrors.PasswordLength(request.Password));
        if (error == PasswordErrors.SpecialCharacters)
            builder.AddError(SignInResponseErrors.PasswordSpecialCharacters(request.Password));
        if (error == PasswordErrors.UpperAndLowerCase)
            builder.AddError(SignInResponseErrors.PasswordUpperAndLowerCase(request.Password));
        if (error == PasswordErrors.Number)
            builder.AddError(SignInResponseErrors.PasswordNumber(request.Password));
        
        return builder.Build();
    }
}
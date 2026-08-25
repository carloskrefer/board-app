using Auth.Application.Commands;
using Auth.Domain.Entities;
using Auth.Domain.Interfaces.Services;

namespace Auth.Application.Mappers.Extensions;

internal static class SignInExtensions
{
    public static Result<User> ToUser(this SignInCommand input, IPasswordHasher hasher)
    {
        return User.Create(input.Email, input.Password, input.Name, hasher);
    }
}
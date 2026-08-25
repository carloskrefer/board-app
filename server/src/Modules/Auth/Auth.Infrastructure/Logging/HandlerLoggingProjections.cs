using Auth.Application.Commands;
using Core.Infrastructure.Logging;

namespace Auth.Infrastructure.Logging;

public static class HandlerLoggingProjections
{
    public static Projections<SignInCommand, SignInOutput> SignInCommand => 
        new Projections<SignInCommand, SignInOutput>
        {
            Input = input => new { input.Email, input.Name },
            Output = output => new { output.UserId }
        };

    public static Projections<LogInCommand, LogInOutput> LogInCommand => 
        new Projections<LogInCommand, LogInOutput>
        {
            Input = input => new { input.Email },
            Output = output => new { output.Email }
        };
}
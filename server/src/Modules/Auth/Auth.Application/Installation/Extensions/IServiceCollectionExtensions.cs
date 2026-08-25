using Auth.Application.Commands;
using Core.Logging.Decorators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Auth.Application.Installation.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        AddSignInCommandHandler(services);
        AddLogInCommandHandler(services);
    }

    private static void AddSignInCommandHandler(this IServiceCollection services)
    {
        services.AddScoped<SignInCommandHandler>();

        services.AddScoped<IHandler<SignInCommand, Result<SignInOutput>>>(sp => {
            var handler = sp.GetRequiredService<SignInCommandHandler>();

            var logger = 
                sp.GetRequiredService<
                    ILogger<LoggingDecorator<SignInCommand, Result<SignInOutput>, SignInOutput>>
                >();
            
            var projections = new Projections<SignInCommand, SignInOutput>
            {
                Input = input => new { input.Email, input.Name },
                Output = output => new { output.UserId }
            };

            return new LoggingDecorator<SignInCommand, Result<SignInOutput>, SignInOutput>(
                logger, 
                handler, 
                "Sign In", 
                projections);
        });
    }

    // TODO: See if I can put this repetitive code in a generic method
    private static void AddLogInCommandHandler(this IServiceCollection services)
    {
        services.AddScoped<LogInCommandHandler>();

        services.AddScoped<IHandler<LogInCommand, Result<LogInOutput>>>(sp => {
            var handler = sp.GetRequiredService<LogInCommandHandler>();

            var logger = 
                sp.GetRequiredService<
                    ILogger<LoggingDecorator<LogInCommand, Result<LogInOutput>, LogInOutput>>
                >();

            var projections = new Projections<LogInCommand, LogInOutput>
            {
                Input = input => new { input.Email },
                Output = output => new { output.Email }
            };
            
            return new LoggingDecorator<LogInCommand, Result<LogInOutput>, LogInOutput>(
                logger, 
                handler, 
                "Log In", 
                projections);
        });
    }
}
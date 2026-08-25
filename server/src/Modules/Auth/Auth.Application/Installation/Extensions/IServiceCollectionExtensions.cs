using Auth.Application.Commands;
using Auth.Domain.Interfaces.Services;
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
            var useCase = sp.GetRequiredService<SignInCommandHandler>();

            var logger = 
                sp.GetRequiredService<
                    ILogger<LoggingDecorator<SignInCommand, Result<SignInOutput>, SignInOutput>>
                >();
            
            return new LoggingDecorator<SignInCommand, Result<SignInOutput>, SignInOutput>(logger, useCase, "Sign In");
        });
    }

    // TODO: See if I can put this repetitive code in a generic method
    private static void AddLogInCommandHandler(this IServiceCollection services)
    {
        services.AddScoped<LogInCommandHandler>();

        services.AddScoped<IHandler<LogInCommand, Result<LogInOutput>>>(sp => {
            var useCase = sp.GetRequiredService<LogInCommandHandler>();

            var logger = 
                sp.GetRequiredService<
                    ILogger<LoggingDecorator<LogInCommand, Result<LogInOutput>, LogInOutput>>
                >();
            
            return new LoggingDecorator<LogInCommand, Result<LogInOutput>, LogInOutput>(logger, useCase, "Log In");
        });
    }
}
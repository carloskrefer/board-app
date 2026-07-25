using Auth.Application.UseCases;
using Auth.Domain.Interfaces.Services;
using Core.Logging.Decorators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Auth.Application.Installation.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        AddSignInService(services);
        AddLogInService(services);
    }

    private static void AddSignInService(this IServiceCollection services)
    {
        services.AddScoped<SignInService>();

        services.AddScoped<IUseCase<SignInInput, Result<SignInOutput>>>(sp => {
            var useCase = sp.GetRequiredService<SignInService>();

            var logger = 
                sp.GetRequiredService<
                    ILogger<LoggingDecorator<SignInInput, Result<SignInOutput>, SignInOutput>>
                >();
            
            return new LoggingDecorator<SignInInput, Result<SignInOutput>, SignInOutput>(logger, useCase, "Sign In");
        });
    }

    // TODO: See if I can put this repetitive code in a generic method
    private static void AddLogInService(this IServiceCollection services)
    {
        services.AddScoped<LogInService>();

        services.AddScoped<IUseCase<LogInInput, Result<LogInOutput>>>(sp => {
            var useCase = sp.GetRequiredService<LogInService>();

            var logger = 
                sp.GetRequiredService<
                    ILogger<LoggingDecorator<LogInInput, Result<LogInOutput>, LogInOutput>>
                >();
            
            return new LoggingDecorator<LogInInput, Result<LogInOutput>, LogInOutput>(logger, useCase, "Log In");
        });
    }
}
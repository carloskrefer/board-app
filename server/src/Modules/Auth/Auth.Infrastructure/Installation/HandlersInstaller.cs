using Auth.Application.Commands;
using Auth.Infrastructure.Logging;
using Core.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infrastructure.Installation;

public static class HandlersInstaller
{
    public static void AddHandlers(this IServiceCollection services)
    {
        AddSignInCommandHandler(services);
        AddLogInCommandHandler(services);
    }

    private static void AddSignInCommandHandler(this IServiceCollection services)
    {
        services.AddScoped<SignInCommandHandler>();

        services.AddScoped(sp => {
            var handler = sp.GetRequiredService<SignInCommandHandler>();

            return handler.DecorateWithLogger<SignInCommandHandler, SignInCommand, SignInOutput>(
                sp, 
                HandlerLoggingProjections.SignInCommand, 
                "Sign In");
        });
    }

    private static void AddLogInCommandHandler(this IServiceCollection services)
    {
        services.AddScoped<LogInCommandHandler>();

        services.AddScoped(sp => {
            var handler = sp.GetRequiredService<LogInCommandHandler>();

            return handler.DecorateWithLogger<LogInCommandHandler, LogInCommand, LogInOutput>(
                sp, 
                HandlerLoggingProjections.LogInCommand, 
                "Log In");
        });
    }
}
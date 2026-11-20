using Auth.Application.Commands;
using Auth.Infrastructure.Logging;
using Core.Infrastructure.Logging;
using Core.Infrastructure.Resilience;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infrastructure.Installation;

public static class HandlersInstaller
{
    public static void AddHandlers(this IServiceCollection services)
    {
        AddSignInCommandHandler(services);
        AddLogInCommandHandler(services);
        AddLogOutCommandHandler(services);
    }

    private static void AddSignInCommandHandler(this IServiceCollection services)
    {
        services.AddScoped<SignInCommandHandler>();

        services.AddScoped(sp => {
            var handler = sp.GetRequiredService<SignInCommandHandler>();
            var handlerName = "Sign In";

            return handler
                .DecorateWithRetrier<SignInCommandHandler, SignInCommand, SignInOutput>(
                    sp,
                    ex => ex is DbUpdateException,
                    handlerName,
                    2)
                .DecorateWithLogger<SignInCommandHandler, SignInCommand, SignInOutput>(
                    sp, 
                    HandlerLoggingProjections.SignInCommand, 
                    handlerName);
        });
    }

    private static void AddLogInCommandHandler(this IServiceCollection services)
    {
        services.AddScoped<LogInCommandHandler>();

        services.AddScoped(sp => {
            var handlerName = "Log In";
            var handler = sp.GetRequiredService<LogInCommandHandler>();

            return handler
                .DecorateWithRetrier<LogInCommandHandler, LogInCommand, LogInOutput>(
                    sp,
                    ex => ex is DbUpdateConcurrencyException,
                    handlerName)
                .DecorateWithLogger<LogInCommandHandler, LogInCommand, LogInOutput>(
                    sp, 
                    HandlerLoggingProjections.LogInCommand, 
                    handlerName);
        });
    }

    private static void AddLogOutCommandHandler(this IServiceCollection services)
    {
        services.AddScoped<LogOutCommandHandler>();

        services.AddScoped(sp => {
            var handler = sp.GetRequiredService<LogOutCommandHandler>();
            var handlerName = "Log Out";

            return handler
                .DecorateWithRetrier<LogOutCommandHandler, LogOutCommand, LogOutOutput>(
                    sp,
                    ex => ex is DbUpdateConcurrencyException,
                    handlerName)
                .DecorateWithLogger<LogOutCommandHandler, LogOutCommand, LogOutOutput>(
                    sp, 
                    HandlerLoggingProjections.LogOutCommand, 
                    handlerName);
        });
    }
}
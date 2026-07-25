using Auth.Infrastructure;

namespace App.Migrations;

public static class MigrationsExtensions
{
    /// <summary>
    /// Applies all the pending migrations for the modules in the application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// You could apply migrations by hand with: 
    /// <code>
    /// dotnet ef database update --project project_name --startup-project ./src/App
    /// </code>
    /// But since there are multiple modules, and each module may have more than one DbContext, it is easier to apply 
    /// them all at once with this method.
    /// </para>
    /// </remarks>
    public static async Task ApplyModulesMigrations(this WebApplication app)
    {
        await app.Services.ApplyAuthModuleMigrationsAsync();
    }
}
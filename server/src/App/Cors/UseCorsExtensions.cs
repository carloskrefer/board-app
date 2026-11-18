namespace App.Cors;

public static class UseCorsExtensions
{
    public static void UseCorsMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.UseCors(PolicyNames.Development);
    }

}
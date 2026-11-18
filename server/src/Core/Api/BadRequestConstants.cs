namespace Core.Api;

public static class BadRequestConstants
{
    public const string Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
    public const string Title = "One or more validation errors occurred.";
    public const string DetailForErrorsExtension = "See the errors property for details.";
}
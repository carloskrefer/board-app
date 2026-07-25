namespace Auth.Domain.ValueObjects;

public sealed record Email(string Value)
{
    public static Result<Email> Create(string value)
    {
        var valueCleaned = value.Trim().ToLowerInvariant();

        if (!System.Net.Mail.MailAddress.TryCreate(valueCleaned, out _))
            return EmailErrors.Format;

        return new Email(valueCleaned);
    }
}

public static class EmailErrors
{
    public static readonly string Id = "EMAIL";
    public static readonly Error Format = 
        new(
            $"{Id}.FORMAT", 
            "Invalid email address format. Expected: user@domain.com.");
}
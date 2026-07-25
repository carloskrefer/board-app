namespace Auth.Infrastructure.Settings;

public class AuthOptions
{
    public const string AppSettingsSection = "Modules:Auth";
    public required JwtOptions Jwt { get; set; }
}

// TODO: I think this file should be in the App module
public class JwtOptions
{
    public const string AppSettingsSection = $"{AuthOptions.AppSettingsSection}:Jwt";
    public required string PrivateKeyPath { get; set; }
    public required string PublicKeyPath { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required int ExpirationInMinutes { get; set; }
}
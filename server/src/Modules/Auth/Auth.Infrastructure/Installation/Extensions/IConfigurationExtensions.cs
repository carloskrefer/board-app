
using System.Security.Cryptography;
using Auth.Infrastructure.Settings;
using Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Infrastructure.Installation.Extensions;

internal static class ConfigurationManagerExtensions
{
    internal static string GetJwtPublicKeyPemText(this IConfiguration configuration)
    {
        var publicKeyPathOptionName = $"{JwtOptions.AppSettingsSection}:{nameof(JwtOptions.PublicKeyPath)}";

        var publicKeyPath = configuration[publicKeyPathOptionName] 
            ?? throw new ConfigurationNotFound(publicKeyPathOptionName);
        
        return File.ReadAllText(publicKeyPath) ?? throw new InvalidDataException("Public key file is empty.");
    }

    internal static string GetJwtPrivateKeyPemText(this IConfiguration configuration)
    {
        var privateKeyPathOptionName = $"{JwtOptions.AppSettingsSection}:{nameof(JwtOptions.PrivateKeyPath)}";

        var privateKeyPath = configuration[privateKeyPathOptionName] 
            ?? throw new ConfigurationNotFound(privateKeyPathOptionName);
        
        return File.ReadAllText(privateKeyPath) ?? throw new InvalidDataException("Private key file is empty.");
    }

    internal static string GetJwtIssuer(this IConfiguration configuration)
    {
        var issuerOptionName = $"{JwtOptions.AppSettingsSection}:{nameof(JwtOptions.Issuer)}";

        return configuration[issuerOptionName] 
            ?? throw new ConfigurationNotFound(issuerOptionName);
    }

    internal static string GetJwtAudience(this IConfiguration configuration)
    {
        var audienceOptionName = $"{JwtOptions.AppSettingsSection}:{nameof(JwtOptions.Audience)}";

        return configuration[audienceOptionName] 
            ?? throw new ConfigurationNotFound(audienceOptionName);
    }

    internal static RsaSecurityKey GetRsaSecurityKey(this IConfiguration configuration)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(configuration.GetJwtPrivateKeyPemText());
        return new RsaSecurityKey(rsa);
    }

    internal static int GetJwtExpirationInMinutes(this IConfiguration configuration)
    {
        var expirationOptionName = $"{JwtOptions.AppSettingsSection}:{nameof(JwtOptions.ExpirationInMinutes)}";

        var value =  int.Parse(configuration[expirationOptionName]
            ?? throw new ConfigurationNotFound(expirationOptionName));

        if (value <= 0)
            throw new InvalidDataException("JWT expiration must be greater than zero.");

        return value;
    }
}
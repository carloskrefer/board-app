
using System.Security.Cryptography;
using Auth.Infrastructure.Settings;
using Core.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace App.DependencyInjection.Extensions;

internal static class ConfigurationManagerExtensions
{
    internal static string GetJwtPublicKeyPemText(this IConfiguration configuration)
    {
        var publicKeyPathOptionName = $"{JwtOptions.AppSettingsSection}:{nameof(JwtOptions.PublicKeyPath)}";

        var publicKeyPath = configuration[publicKeyPathOptionName] 
            ?? throw new ConfigurationNotFound(publicKeyPathOptionName);
        
        return File.ReadAllText(publicKeyPath)
            ?? throw new InvalidDataException("Public key file is empty.");
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
        rsa.ImportFromPem(configuration.GetJwtPublicKeyPemText());
        return new RsaSecurityKey(rsa);
    }
}
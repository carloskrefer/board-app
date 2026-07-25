using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.Application.Interfaces.Services;
using Auth.Infrastructure.Installation.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Infrastructure.ApplicationServices;

public class JwtService : ICredentialsService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public string GenerateSerializedCredentials(Guid userId, string email, string name)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.UniqueName, name),
        };

        var signingCredentials = new SigningCredentials(
            _configuration.GetRsaSecurityKey(),
            SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration.GetJwtIssuer(),
            audience: _configuration.GetJwtAudience(),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_configuration.GetJwtExpirationInMinutes()),
            notBefore: DateTime.UtcNow,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

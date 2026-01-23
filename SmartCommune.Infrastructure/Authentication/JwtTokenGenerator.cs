using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using SmartCommune.Application.Common.Constants;
using SmartCommune.Application.Common.Interfaces.Authentication;
using SmartCommune.Application.Common.Options;
using SmartCommune.Domain.UserAggregate;

namespace SmartCommune.Infrastructure.Authentication;

public class JwtTokenGenerator(
    IOptions<JwtSettings> jwtSettingsOption)
    : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = jwtSettingsOption.Value;

    public string GenerateAccessToken(ApplicationUser user)
    {
        var signingCredentials = new SigningCredentials(
                                    new SymmetricSecurityKey(
                                        Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                                    SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
            new(JwtRegisteredClaimNames.Name, user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            new(CustomClaims.SecurityStamp, user.SecurityStamp.ToString()),
            new(CustomClaims.RoleId, user.RoleId.Value.ToString()),
        };

        var securityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            audience: _jwtSettings.Audience,
            claims: claims,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }
}
using Iskra.Core.Domain.Entities;
using Iskra.Modules.Auth.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Iskra.Modules.Auth.Services;

internal sealed class JwtProvider(IOptions<AuthOptions> options)
{
    private readonly JwtOptions _jwtOptions = options.Value.Jwt;

    public string GenerateAccessToken(User user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generates a cryptographically secure random token.
    /// Uses 64 bytes (512 bits) of entropy and converts to a Hex string.
    /// </summary>
    public string GenerateRefreshTokenString()
    {
        // Allocate on stack to avoid GC pressure for short-lived buffer
        Span<byte> randomNumber = stackalloc byte[64];

        // Use the system's CSPRNG (Cryptographically Secure Pseudo-Random Number Generator)
        RandomNumberGenerator.Fill(randomNumber);

        // Convert to Hex String (e.g., "A1B2C3...")
        // Length will be 128 characters (2 chars per byte)
        return Convert.ToHexString(randomNumber);
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shopizy.Infrastructure.Security.TokenGenerator;

namespace Adda.API.Security.TokenGenerator;

public class JwtTokenGenerator(IOptions<JwtSettings> jwtOptoins) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = jwtOptoins.Value;

    public string GenerateToken(
        int userId,
        string username,
        IList<string> roles
    )
    {

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, username),
        };

        foreach (string role in roles)
        {
            claims.Add(new(ClaimTypes.Role, role));
        }

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            SecurityAlgorithms.HmacSha256Signature
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationMinutes),
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = creds,
        };

        var jwtTokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = jwtTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        string token = jwtTokenHandler.WriteToken(jwtToken);
        return token;
    }
}

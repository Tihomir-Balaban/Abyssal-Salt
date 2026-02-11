using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AbySalto.Mid.Application.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AbySalto.Mid.Infrastructure.Security;

public sealed class TokenService(IConfiguration config) : ITokenService
{
    public string CreateAccessToken(Guid userId, string email)
    {
        var key = config["Jwt:Key"];
        var issuer = config["Jwt:Issuer"];
        var audience = config["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Missing JWT configuration: Jwt:Key");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
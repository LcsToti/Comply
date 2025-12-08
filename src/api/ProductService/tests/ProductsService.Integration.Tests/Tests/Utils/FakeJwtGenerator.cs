using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductsService.Integration.Tests.Tests.Utils;

public static class FakeJwtGenerator
{
    public static string GenerateJwt(IConfiguration config, string role, Guid? userId = null)
    {
        var secret = config["JwtSettings:Secret"];
        var issuer = config["JwtSettings:Issuer"];
        var audience = config["JwtSettings:Audience"];

        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, (userId ?? Guid.NewGuid()).ToString()),
            new Claim(ClaimTypes.Role, role)
        };

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return handler.WriteToken(token);
    }
}
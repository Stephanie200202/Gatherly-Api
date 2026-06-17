using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gatherly.Application.Abstractions;
using Gatherly.Application.Configuration;
using Gatherly.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Gatherly.Infrastructure.HelperServices;

public class JwtProvider : IJwtProvider
{
    private readonly JwtSettings _jwtSettings;

    public JwtProvider(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

 
    public TokenResult GenerateToken(ApplicationUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
{
    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
    new Claim(ClaimTypes.Role, user.Role.ToString()),
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
};

        if (!string.IsNullOrEmpty(user.Email))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        // Generate a random string payload for the refresh token channel
        var refreshToken = Guid.NewGuid().ToString("N");

        // Adjust properties below to match fields inside your actual TokenResult definition
        return new TokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60
        };
    }

    // 2. Matches your interface signature: GetPrincipalFromExpiredToken
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ValidateLifetime = false // Crucial: We need to read claims from an already expired token
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null; 
        }
    }
}
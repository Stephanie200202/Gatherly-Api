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
    private readonly IConfiguration _configuration; 


    public JwtProvider(IOptions<JwtSettings> jwtSettings, IConfiguration configuration)
    {
        _jwtSettings = jwtSettings.Value;
        _configuration = configuration;
    }

    public TokenResult GenerateToken(ApplicationUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

  
        var secretKey = _configuration["Jwt:SecretKey"] ?? _jwtSettings.SecretKey;
        var key = Encoding.UTF8.GetBytes(secretKey);

        string userRole = user.Role.ToString();
        string normalizedRole = !string.IsNullOrEmpty(userRole)
            ? char.ToUpper(userRole[0]) + userRole.Substring(1).ToLower()
            : string.Empty;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("role", normalizedRole)
        };

        if (!string.IsNullOrEmpty(user.Email))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = _configuration["Jwt:Issuer"] ?? _jwtSettings.Issuer,
            Audience = _configuration["Jwt:Audience"] ?? _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);
        var refreshToken = Guid.NewGuid().ToString("N");

        return new TokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600
        };
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
  
        return null;
    }
}
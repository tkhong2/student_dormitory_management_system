using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BillingMaintenanceService.Infrastructure.Auth
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JwtTokenResultDto GenerateToken(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Key"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];

            var expiresMinutes = 60;
            if (int.TryParse(jwtSection["ExpiresMinutes"], out var parsedExpiresMinutes) && parsedExpiresMinutes > 0)
                expiresMinutes = parsedExpiresMinutes;

            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("JWT Key is not configured");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("username", user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("role", user.Role.ToString())
            };

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            return new JwtTokenResultDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = token.ValidTo
            };
        }
    }
}

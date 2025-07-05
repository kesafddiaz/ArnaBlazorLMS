using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ArnaBlazorTest.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;

namespace ArnaBlazorTest.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtService> _logger;

    public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateToken(User user)
    {
        try
        {
            _logger.LogInformation("Generating JWT token for user: {Username}", user.Username);
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.UserRole.Name),
                new Claim("ManagerId", user.ManagerId?.ToString() ?? "0")
            };

            _logger.LogInformation("JWT claims created for user {Username}: Id={UserId}, Role={Role}", user.Username, user.Id, user.UserRole.Name);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogInformation("JWT token generated successfully for user: {Username}", user.Username);
            
            return tokenString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating JWT token for user: {Username}", user.Username);
            throw;
        }
    }
} 
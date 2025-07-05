using Microsoft.AspNetCore.Mvc;
using ArnaBlazorTest.Services;
using Microsoft.Extensions.Logging;
using ArnaBlazorTest.Data;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace ArnaBlazorTest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly ApplicationDbContext _context;

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public int? ManagerId { get; set; }
    }

    public AuthController(IAuthService authService, ILogger<AuthController> logger, ApplicationDbContext context)
    {
        _authService = authService;
        _logger = logger;
        _context = context;
    }

    [HttpGet("debug/users")]
    public IActionResult DebugUsers()
    {
        var users = _context.Users.Include(u => u.UserRole).ToList();
        var userList = users.Select(u => new
        {
            u.Id,
            u.Username,
            u.Email,
            u.RoleId,
            RoleName = u.UserRole?.Name,
            u.CreatedAt
        }).ToList();
        
        return Ok(new { 
            TotalUsers = users.Count,
            Users = userList 
        });
    }

    [HttpGet("debug/test-password")]
    public IActionResult TestPassword()
    {
        var testPassword = "password123";
        var hash = BCrypt.Net.BCrypt.HashPassword(testPassword);
        var verifyResult = BCrypt.Net.BCrypt.Verify(testPassword, hash);
        
        return Ok(new
        {
            OriginalPassword = testPassword,
            Hash = hash,
            VerificationResult = verifyResult
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for username: {Username}", request.Username);
        
        var result = await _authService.LoginAsync(request.Username, request.Password);

        if (!result.success)
        {
            _logger.LogWarning("Login failed for username: {Username}, message: {Message}", request.Username, result.message);
            return BadRequest(new { message = result.message });
        }

        _logger.LogInformation("Login successful for username: {Username}", request.Username);
        return Ok(new { token = result.token, user = result.user });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Registration attempt for username: {Username}", request.Username);
        
        var result = await _authService.RegisterAsync(
            request.Username,
            request.Password,
            request.Email,
            request.RoleId,
            request.ManagerId
        );

        if (!result.success)
        {
            _logger.LogWarning("Registration failed for username: {Username}, message: {Message}", request.Username, result.message);
            return BadRequest(new { message = result.message });
        }

        _logger.LogInformation("Registration successful for username: {Username}", request.Username);
        return Ok(new { message = result.message, user = result.user });
    }
} 
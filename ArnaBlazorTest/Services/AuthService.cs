using ArnaBlazorTest.Data;
using ArnaBlazorTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArnaBlazorTest.Services;

public interface IAuthService
{
    Task<(bool success, string message, User? user)> RegisterAsync(string username, string password, string email, int roleId, int? managerId);
    Task<(bool success, string message, string? token, User? user)> LoginAsync(string username, string password);
    Task<User?> GetUserByIdAsync(int userId);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, IJwtService jwtService, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<(bool success, string message, User? user)> RegisterAsync(string username, string password, string email, int roleId, int? managerId)
    {
        _logger.LogInformation("Attempting to register user: {Username}", username);
        
        var existingByUsername = await _userRepository.FindAsync(u => u.Username == username);
        if (existingByUsername.Any())
        {
            _logger.LogWarning("Registration failed: Username {Username} already exists", username);
            return (false, "Username already exists", null);
        }

        var existingByEmail = await _userRepository.FindAsync(u => u.Email == email);
        if (existingByEmail.Any())
        {
            _logger.LogWarning("Registration failed: Email {Email} already exists", email);
            return (false, "Email already exists", null);
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        _logger.LogDebug("Password hashed successfully for user: {Username}", username);

        var user = new User
        {
            Username = username,
            PasswordHash = passwordHash,
            Email = email,
            RoleId = roleId,
            ManagerId = managerId
        };

        await _userRepository.AddAsync(user);
        _logger.LogInformation("User registered successfully: {Username}", username);
        return (true, "Registration successful", user);
    }

    public async Task<(bool success, string message, string? token, User? user)> LoginAsync(string username, string password)
    {
        _logger.LogInformation("Login attempt for user: {Username}", username);
        
        var user = await _userRepository.GetByUsernameAsync(username);
        _logger.LogInformation("User lookup result for {Username}: {UserFound}", username, user != null ? "Found" : "Not Found");
        
        if (user == null)
        {
            _logger.LogWarning("Login failed: User {Username} not found", username);
            return (false, "Invalid username or password", null, null);
        }

        _logger.LogInformation("User found: {Username}, verifying password", username);
        _logger.LogInformation("User details: Id={UserId}, Email={Email}, RoleId={RoleId}", user.Id, user.Email, user.RoleId);
        
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed: Invalid password for user {Username}", username);
            return (false, "Invalid username or password", null, null);
        }

        _logger.LogInformation("Password verified successfully for user: {Username}", username);
        
        var token = _jwtService.GenerateToken(user);
        _logger.LogInformation("Login successful for user: {Username}", username);
        return (true, "Login successful", token, user);
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _userRepository.GetWithRoleAndProgressAsync(userId);
    }
} 
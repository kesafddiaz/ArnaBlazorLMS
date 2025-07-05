using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArnaBlazorTest.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
    private readonly IAuthService _authService;
    private readonly ILogger<CustomAuthStateProvider> _logger;

    public CustomAuthStateProvider(IAuthService authService, ILogger<CustomAuthStateProvider> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _logger.LogInformation("GetAuthenticationStateAsync called, user authenticated: {IsAuthenticated}", _currentUser.Identity?.IsAuthenticated);
        return Task.FromResult(new AuthenticationState(_currentUser));
    }

    public Task UpdateAuthenticationState(string? token)
    {
        _logger.LogInformation("Updating authentication state with token: {HasToken}", !string.IsNullOrEmpty(token));
        
        ClaimsPrincipal claimsPrincipal;

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogInformation("Setting anonymous authentication state");
            claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        }
        else
        {
            try
            {
                _logger.LogInformation("Parsing JWT token and creating claims principal");
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
                claimsPrincipal = new ClaimsPrincipal(identity);
                _logger.LogInformation("Claims principal created successfully with {ClaimCount} claims", jwtToken.Claims.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing JWT token");
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            }
        }

        // Store the current user
        _currentUser = claimsPrincipal;
        
        _logger.LogInformation("Notifying authentication state changed");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        return Task.CompletedTask;
    }
} 
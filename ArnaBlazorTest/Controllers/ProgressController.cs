using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ArnaBlazorTest.Services;

namespace ArnaBlazorTest.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProgressController : ControllerBase
{
    private readonly IProgressService _progressService;

    public ProgressController(IProgressService progressService)
    {
        _progressService = progressService;
    }

    [HttpGet("team")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetTeamProgress()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int managerId))
        {
            return Unauthorized();
        }

        var teamProgress = await _progressService.GetTeamProgressReportAsync(managerId);
        return Ok(teamProgress);
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserProgress()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized();
        }

        try
        {
            var userProgress = await _progressService.GetUserProgressReportAsync(userId);
            return Ok(userProgress);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetUserProgressById(int userId)
    {
        var managerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (managerIdClaim == null || !int.TryParse(managerIdClaim.Value, out int managerId))
        {
            return Unauthorized();
        }

        try
        {
            var userProgress = await _progressService.GetUserProgressReportAsync(userId);
            
            // Verify that the user is a subordinate of the manager
            if (userProgress.User.ManagerId != managerId)
            {
                return Forbid();
            }

            return Ok(userProgress);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("assignments")]
    public async Task<IActionResult> GetUserAssignmentProgresses()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized();
        }

        var progresses = await _progressService.GetUserAssignmentProgressesAsync(userId);
        return Ok(progresses);
    }

    [HttpGet("assignment/{assignmentId}")]
    public async Task<IActionResult> GetAssignmentProgress(int assignmentId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized();
        }

        var progress = await _progressService.GetAssignmentProgressAsync(userId, assignmentId);
        if (progress == null)
        {
            return NotFound();
        }

        return Ok(progress);
    }
} 
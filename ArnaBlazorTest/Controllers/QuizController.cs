using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ArnaBlazorTest.Services;

namespace ArnaBlazorTest.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;

    public class SubmitQuizRequest
    {
        public int AssignmentId { get; set; }
        public Dictionary<int, string> Answers { get; set; } = new();
    }

    public QuizController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitQuiz([FromBody] SubmitQuizRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized();
        }

        // Check if user has already submitted
        var hasSubmitted = await _quizService.HasUserSubmittedQuiz(userId, request.AssignmentId);
        if (hasSubmitted)
        {
            return BadRequest(new { message = "Quiz already submitted" });
        }

        var (score, message) = await _quizService.SubmitQuizAsync(userId, request.AssignmentId, request.Answers);
        return Ok(new { score, message });
    }

    [HttpGet("{assignmentId}/status")]
    public async Task<IActionResult> GetQuizStatus(int assignmentId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized();
        }

        var hasSubmitted = await _quizService.HasUserSubmittedQuiz(userId, assignmentId);
        return Ok(new { hasSubmitted });
    }

    [HttpGet("{assignmentId}/answers")]
    public async Task<IActionResult> GetQuizAnswers(int assignmentId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized();
        }

        var answers = await _quizService.GetUserAnswers(userId, assignmentId);
        if (answers == null)
        {
            return NotFound();
        }

        return Ok(answers);
    }
} 
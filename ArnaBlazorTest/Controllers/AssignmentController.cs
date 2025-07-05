using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ArnaBlazorTest.Services;
using ArnaBlazorTest.Models;

namespace ArnaBlazorTest.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssignmentController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;

    public class CreateAssignmentRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MaterialUrl { get; set; } = string.Empty;
        public List<Question> Questions { get; set; } = new();
    }

    public AssignmentController(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAssignments()
    {
        var assignments = await _assignmentService.GetAllAssignmentsAsync();
        return Ok(assignments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssignment(int id)
    {
        var assignment = await _assignmentService.GetAssignmentAsync(id);
        if (assignment == null)
            return NotFound();

        return Ok(assignment);
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentRequest request)
    {
        var assignment = new Assignment
        {
            Title = request.Title,
            Description = request.Description,
            MaterialUrl = request.MaterialUrl,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            Questions = request.Questions
        };

        try
        {
            var createdAssignment = await _assignmentService.CreateAssignmentAsync(assignment);
            return CreatedAtAction(nameof(GetAssignment), new { id = createdAssignment.Id }, createdAssignment);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to create assignment", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> UpdateAssignment(int id, [FromBody] Assignment assignment)
    {
        if (id != assignment.Id)
            return BadRequest();

        try
        {
            var updatedAssignment = await _assignmentService.UpdateAssignmentAsync(id, assignment);
            return Ok(updatedAssignment);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to update assignment", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> DeleteAssignment(int id)
    {
        try
        {
            await _assignmentService.DeleteAssignmentAsync(id);
            return NoContent();
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to delete assignment", error = ex.Message });
        }
    }

    [HttpGet("{id}/questions")]
    public async Task<IActionResult> GetAssignmentQuestions(int id)
    {
        var questions = await _assignmentService.GetQuestionsForAssignmentAsync(id);
        return Ok(questions);
    }
} 
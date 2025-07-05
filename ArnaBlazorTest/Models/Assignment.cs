namespace ArnaBlazorTest.Models;

public class Assignment
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string? MaterialUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public List<Question> Questions { get; set; } = new();
    public ICollection<AssignmentProgress> AssignmentProgresses { get; set; } = new List<AssignmentProgress>();
}

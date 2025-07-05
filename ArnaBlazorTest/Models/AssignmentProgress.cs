using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArnaBlazorTest.Models;

public class AssignmentProgress
{
    public int Id { get; set; }
    public int Score { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public string Status { get; set; } = "pending"; // pending, completed
    public string AnswersJson { get; set; } = "{}";
    
    // Foreign keys
    public int UserId { get; set; }
    public int AssignmentId { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Assignment Assignment { get; set; } = null!;
    
    // Helper property
    [NotMapped]
    public Dictionary<int, string> Answers
    {
        get => JsonSerializer.Deserialize<Dictionary<int, string>>(AnswersJson) ?? new Dictionary<int, string>();
        set => AnswersJson = JsonSerializer.Serialize(value);
    }
} 
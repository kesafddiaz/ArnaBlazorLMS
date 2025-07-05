using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArnaBlazorTest.Models;

public class Question
{
    public int Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string OptionsJson { get; set; } = "[]";
    public string CorrectAnswer { get; set; } = string.Empty;
    
    // Foreign key
    public int AssignmentId { get; set; }
    
    // Navigation property
    public Assignment Assignment { get; set; } = null!;
    
    // Helper properties
    [NotMapped]
    public List<string> Options
    {
        get => JsonSerializer.Deserialize<List<string>>(OptionsJson) ?? new List<string>();
        set => OptionsJson = JsonSerializer.Serialize(value);
    }
} 
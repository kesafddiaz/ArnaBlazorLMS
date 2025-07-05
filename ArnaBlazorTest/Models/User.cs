namespace ArnaBlazorTest.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign keys
    public int RoleId { get; set; }
    public int? ManagerId { get; set; }
    
    // Navigation properties
    public UserRole UserRole { get; set; } = null!;
    public User? Manager { get; set; }
    public ICollection<User> Subordinates { get; set; } = new List<User>();
    public ICollection<AssignmentProgress> AssignmentProgresses { get; set; } = new List<AssignmentProgress>();
} 
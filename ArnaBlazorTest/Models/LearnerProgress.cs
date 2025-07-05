namespace ArnaBlazorTest.Models;

public class LearnerProgress
{
    public User User { get; set; } = null!;
    public int CompletedAssignments { get; set; }
    public int TotalAssignments { get; set; }
    public double AverageScore { get; set; }
    public DateTime? LastActivityDate { get; set; }
}

public class LearnerDetailReport : LearnerProgress
{
    public List<WeeklyStat> WeeklyStats { get; set; } = new();
    public List<AssignmentProgress> AssignmentProgresses { get; set; } = new();
}

public class WeeklyStat
{
    public DateTime WeekStart { get; set; }
    public int CompletedCount { get; set; }
    public double AverageScore { get; set; }
} 
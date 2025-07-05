using ArnaBlazorTest.Data;
using ArnaBlazorTest.Models;
using Microsoft.EntityFrameworkCore;

namespace ArnaBlazorTest.Services;

public class UserProgressReport
{
    public User User { get; set; } = null!;
    public List<AssignmentProgress> Progresses { get; set; } = new();
    public int CompletedAssignments { get; set; }
    public int TotalAssignments { get; set; }
    public double AverageScore { get; set; }
}

public interface IProgressService
{
    Task<List<UserProgressReport>> GetTeamProgressReportAsync(int managerId);
    Task<UserProgressReport> GetUserProgressReportAsync(int userId);
    Task<List<AssignmentProgress>> GetUserAssignmentProgressesAsync(int userId);
    Task<AssignmentProgress?> GetAssignmentProgressAsync(int userId, int assignmentId);
    Task<LearnerDetailReport?> GetLearnerDetailReportAsync(int userId);
    Task<List<LearnerProgress>> GetAllLearnersProgressAsync(int managerId);
}

public class ProgressService : IProgressService
{
    private readonly IProgressRepository _progressRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAssignmentRepository _assignmentRepository;

    public ProgressService(IProgressRepository progressRepository, IUserRepository userRepository, IAssignmentRepository assignmentRepository)
    {
        _progressRepository = progressRepository;
        _userRepository = userRepository;
        _assignmentRepository = assignmentRepository;
    }

    public async Task<List<UserProgressReport>> GetTeamProgressReportAsync(int managerId)
    {
        var reports = new List<UserProgressReport>();
        var totalAssignments = (await _assignmentRepository.GetAllAsync()).Count(a => a.IsActive);
        var subordinates = (await _userRepository.FindAsync(u => u.ManagerId == managerId)).ToList();
        
        foreach (var user in subordinates)
        {
            // Get user with progress data
            var userWithProgress = await _userRepository.GetWithRoleAndProgressAsync(user.Id);
            if (userWithProgress != null)
            {
                var report = CreateUserProgressReport(userWithProgress, totalAssignments);
                reports.Add(report);
            }
        }
        return reports;
    }

    public async Task<UserProgressReport> GetUserProgressReportAsync(int userId)
    {
        var user = await _userRepository.GetWithRoleAndProgressAsync(userId)
            ?? throw new ArgumentException("User not found");
        var totalAssignments = (await _assignmentRepository.GetAllAsync()).Count(a => a.IsActive);
        return CreateUserProgressReport(user, totalAssignments);
    }

    public async Task<List<AssignmentProgress>> GetUserAssignmentProgressesAsync(int userId)
    {
        return await _progressRepository.GetByUserIdAsync(userId);
    }

    public async Task<AssignmentProgress?> GetAssignmentProgressAsync(int userId, int assignmentId)
    {
        return await _progressRepository.GetByUserAndAssignmentAsync(userId, assignmentId);
    }

    private UserProgressReport CreateUserProgressReport(User user, int totalAssignments)
    {
        var completedAssignments = user.AssignmentProgresses.Count(ap => ap.Status == "completed");
        var averageScore = user.AssignmentProgresses.Any() 
            ? user.AssignmentProgresses.Average(ap => ap.Score) 
            : 0;

        return new UserProgressReport
        {
            User = user,
            Progresses = user.AssignmentProgresses.ToList(),
            CompletedAssignments = completedAssignments,
            TotalAssignments = totalAssignments,
            AverageScore = averageScore
        };
    }

    public async Task<LearnerDetailReport?> GetLearnerDetailReportAsync(int userId)
    {
        var user = await _userRepository.GetWithRoleAndProgressAsync(userId);

        if (user == null)
            return null;

        var totalAssignments = (await _assignmentRepository.GetAllAsync()).Count();

        var report = new LearnerDetailReport
        {
            User = user,
            CompletedAssignments = user.AssignmentProgresses.Count(ap => ap.Status == "completed"),
            TotalAssignments = totalAssignments,
            AverageScore = user.AssignmentProgresses.Any() 
                ? user.AssignmentProgresses.Average(ap => ap.Score) 
                : 0,
            LastActivityDate = user.AssignmentProgresses
                .OrderByDescending(ap => ap.SubmittedAt)
                .FirstOrDefault()?.SubmittedAt,
            AssignmentProgresses = user.AssignmentProgresses.ToList()
        };

        // Calculate weekly stats
        var weeklyStats = user.AssignmentProgresses
            .Where(ap => ap.SubmittedAt.HasValue)
            .GroupBy(ap => GetWeekStart(ap.SubmittedAt!.Value))
            .Select(g => new WeeklyStat
            {
                WeekStart = g.Key,
                CompletedCount = g.Count(),
                AverageScore = g.Average(ap => ap.Score)
            })
            .OrderByDescending(ws => ws.WeekStart)
            .ToList();

        report.WeeklyStats = weeklyStats;

        return report;
    }

    public async Task<List<LearnerProgress>> GetAllLearnersProgressAsync(int managerId)
    {
        var learners = (await _userRepository.FindAsync(u => u.ManagerId == managerId)).ToList();
        var totalAssignments = (await _assignmentRepository.GetAllAsync()).Count();
        return learners.Select(l => new LearnerProgress
        {
            User = l,
            CompletedAssignments = l.AssignmentProgresses.Count(ap => ap.Status == "completed"),
            TotalAssignments = totalAssignments,
            AverageScore = l.AssignmentProgresses.Any()
                ? l.AssignmentProgresses.Average(ap => ap.Score)
                : 0,
            LastActivityDate = l.AssignmentProgresses
                .OrderByDescending(ap => ap.SubmittedAt)
                .FirstOrDefault()?.SubmittedAt
        }).ToList();
    }

    private static DateTime GetWeekStart(DateTime date)
    {
        var diff = date.DayOfWeek - DayOfWeek.Monday;
        if (diff < 0)
            diff += 7;
        return date.Date.AddDays(-diff);
    }
} 
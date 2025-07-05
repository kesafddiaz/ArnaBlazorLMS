using ArnaBlazorTest.Data;
using ArnaBlazorTest.Models;
using Microsoft.EntityFrameworkCore;

namespace ArnaBlazorTest.Services;

public interface IQuizService
{
    Task<(int score, string message)> SubmitQuizAsync(int userId, int assignmentId, Dictionary<int, string> answers);
    Task<bool> HasUserSubmittedQuiz(int userId, int assignmentId);
    Task<Dictionary<int, string>?> GetUserAnswers(int userId, int assignmentId);
    Task<int> GetUserScore(int userId, int assignmentId);
}

public class QuizService : IQuizService
{
    private readonly IQuizRepository _quizRepository;
    private const int POINTS_PER_QUESTION = 20;

    public QuizService(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public async Task<(int score, string message)> SubmitQuizAsync(int userId, int assignmentId, Dictionary<int, string> answers)
    {
        // Check if user has already submitted
        var existingProgress = await _quizRepository.FindAsync(ap => ap.UserId == userId && ap.AssignmentId == assignmentId);
        var existing = existingProgress.FirstOrDefault();

        if (existing?.Status == "completed")
        {
            return (existing.Score, "Quiz already submitted");
        }

        // Get questions and calculate score
        var questions = await _quizRepository.GetQuestionsForAssignmentAsync(assignmentId);

        if (!questions.Any())
        {
            return (0, "No questions found for this assignment");
        }

        int score = 0;
        foreach (var question in questions)
        {
            if (answers.TryGetValue(question.Id, out string? answer) && 
                answer.Equals(question.CorrectAnswer, StringComparison.OrdinalIgnoreCase))
            {
                score += POINTS_PER_QUESTION;
            }
        }

        // Create or update progress
        if (existing == null)
        {
            existing = new AssignmentProgress
            {
                UserId = userId,
                AssignmentId = assignmentId,
                Score = score,
                Status = "completed",
                SubmittedAt = DateTime.UtcNow,
                Answers = answers
            };
            await _quizRepository.AddAsync(existing);
        }
        else
        {
            existing.Score = score;
            existing.Status = "completed";
            existing.SubmittedAt = DateTime.UtcNow;
            existing.Answers = answers;
            await _quizRepository.UpdateAsync(existing);
        }

        return (score, "Quiz submitted successfully");
    }

    public async Task<bool> HasUserSubmittedQuiz(int userId, int assignmentId)
    {
        return await _quizRepository.HasUserSubmittedQuizAsync(userId, assignmentId);
    }

    public async Task<Dictionary<int, string>?> GetUserAnswers(int userId, int assignmentId)
    {
        return await _quizRepository.GetUserAnswersAsync(userId, assignmentId);
    }

    public async Task<int> GetUserScore(int userId, int assignmentId)
    {
        var progress = await _quizRepository.FindAsync(ap => ap.UserId == userId && ap.AssignmentId == assignmentId);
        return progress.FirstOrDefault()?.Score ?? 0;
    }
} 
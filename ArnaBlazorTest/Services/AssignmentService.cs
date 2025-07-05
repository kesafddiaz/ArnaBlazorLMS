using ArnaBlazorTest.Data;
using ArnaBlazorTest.Models;
using Microsoft.EntityFrameworkCore;

namespace ArnaBlazorTest.Services;

public interface IAssignmentService
{
    Task<List<Assignment>> GetAllAssignmentsAsync();
    Task<Assignment?> GetAssignmentAsync(int id);
    Task<Assignment> CreateAssignmentAsync(Assignment assignment);
    Task<Assignment> UpdateAssignmentAsync(int id, Assignment assignment);
    Task DeleteAssignmentAsync(int id);
    Task<List<Question>> GetQuestionsForAssignmentAsync(int assignmentId);
}

public class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _assignmentRepository;

    public AssignmentService(IAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<List<Assignment>> GetAllAssignmentsAsync()
    {
        return await _assignmentRepository.GetAllWithQuestionsAsync();
    }

    public async Task<Assignment?> GetAssignmentAsync(int id)
    {
        return await _assignmentRepository.GetWithQuestionsAsync(id);
    }

    public async Task<Assignment> CreateAssignmentAsync(Assignment assignment)
    {
        await _assignmentRepository.AddAsync(assignment);
        return assignment;
    }

    public async Task<Assignment> UpdateAssignmentAsync(int id, Assignment assignment)
    {
        var existingAssignment = await _assignmentRepository.GetWithQuestionsAsync(id)
            ?? throw new ArgumentException("Assignment not found");

        existingAssignment.Title = assignment.Title;
        existingAssignment.Description = assignment.Description;
        existingAssignment.MaterialUrl = assignment.MaterialUrl;
        existingAssignment.IsActive = assignment.IsActive;

        // Update questions
        existingAssignment.Questions.Clear();
        foreach (var q in assignment.Questions)
        {
            existingAssignment.Questions.Add(q);
        }

        await _assignmentRepository.UpdateAsync(existingAssignment);
        return existingAssignment;
    }

    public async Task DeleteAssignmentAsync(int id)
    {
        await _assignmentRepository.DeleteAsync(id);
    }

    public async Task<List<Question>> GetQuestionsForAssignmentAsync(int assignmentId)
    {
        var assignment = await _assignmentRepository.GetWithQuestionsAsync(assignmentId);
        return assignment?.Questions ?? new List<Question>();
    }
} 
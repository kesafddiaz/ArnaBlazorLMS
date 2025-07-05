using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ArnaBlazorTest.Models;
using ArnaBlazorTest.Data;
using System.Linq.Expressions;
using System;
using System.Linq;

namespace ArnaBlazorTest.Services
{
    public interface IQuizRepository : IRepository<AssignmentProgress>
    {
        Task<bool> HasUserSubmittedQuizAsync(int userId, int assignmentId);
        Task<Dictionary<int, string>?> GetUserAnswersAsync(int userId, int assignmentId);
        Task<List<Question>> GetQuestionsForAssignmentAsync(int assignmentId);
    }

    public class QuizRepository : IQuizRepository
    {
        private readonly ApplicationDbContext _context;
        public QuizRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AssignmentProgress?> GetByIdAsync(int id)
        {
            return await _context.AssignmentProgresses.FindAsync(id);
        }

        public async Task<IEnumerable<AssignmentProgress>> GetAllAsync()
        {
            return await _context.AssignmentProgresses.ToListAsync();
        }

        public async Task AddAsync(AssignmentProgress entity)
        {
            await _context.AssignmentProgresses.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AssignmentProgress entity)
        {
            _context.AssignmentProgresses.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.AssignmentProgresses.FindAsync(id);
            if (entity != null)
            {
                _context.AssignmentProgresses.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<AssignmentProgress>> FindAsync(Expression<Func<AssignmentProgress, bool>> predicate)
        {
            return await _context.AssignmentProgresses.Where(predicate).ToListAsync();
        }

        public async Task<bool> HasUserSubmittedQuizAsync(int userId, int assignmentId)
        {
            return await _context.AssignmentProgresses
                .AnyAsync(ap => ap.UserId == userId && 
                               ap.AssignmentId == assignmentId && 
                               ap.Status == "completed");
        }

        public async Task<Dictionary<int, string>?> GetUserAnswersAsync(int userId, int assignmentId)
        {
            var progress = await _context.AssignmentProgresses
                .FirstOrDefaultAsync(ap => ap.UserId == userId && 
                                         ap.AssignmentId == assignmentId);

            return progress?.Answers;
        }

        public async Task<List<Question>> GetQuestionsForAssignmentAsync(int assignmentId)
        {
            return await _context.Questions
                .Where(q => q.AssignmentId == assignmentId)
                .ToListAsync();
        }
    }
} 
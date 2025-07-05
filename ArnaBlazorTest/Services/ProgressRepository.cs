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
    public interface IProgressRepository : IRepository<AssignmentProgress>
    {
        Task<List<AssignmentProgress>> GetByUserIdAsync(int userId);
        Task<AssignmentProgress?> GetByUserAndAssignmentAsync(int userId, int assignmentId);
        Task<List<AssignmentProgress>> GetByManagerIdAsync(int managerId);
    }

    public class ProgressRepository : IProgressRepository
    {
        private readonly ApplicationDbContext _context;
        public ProgressRepository(ApplicationDbContext context)
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

        public async Task<List<AssignmentProgress>> GetByUserIdAsync(int userId)
        {
            return await _context.AssignmentProgresses
                .Include(ap => ap.Assignment)
                .Where(ap => ap.UserId == userId)
                .OrderByDescending(ap => ap.SubmittedAt)
                .ToListAsync();
        }

        public async Task<AssignmentProgress?> GetByUserAndAssignmentAsync(int userId, int assignmentId)
        {
            return await _context.AssignmentProgresses
                .Include(ap => ap.Assignment)
                .FirstOrDefaultAsync(ap => ap.UserId == userId && ap.AssignmentId == assignmentId);
        }

        public async Task<List<AssignmentProgress>> GetByManagerIdAsync(int managerId)
        {
            return await _context.AssignmentProgresses
                .Include(ap => ap.User)
                .Where(ap => ap.User.ManagerId == managerId)
                .ToListAsync();
        }
    }
} 
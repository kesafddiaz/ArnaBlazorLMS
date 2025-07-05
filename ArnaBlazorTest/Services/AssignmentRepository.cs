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
    public interface IAssignmentRepository : IRepository<Assignment>
    {
        Task<List<Assignment>> GetAllWithQuestionsAsync();
        Task<Assignment?> GetWithQuestionsAsync(int id);
    }

    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly ApplicationDbContext _context;
        public AssignmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Assignment?> GetByIdAsync(int id)
        {
            return await _context.Assignments.FindAsync(id);
        }

        public async Task<IEnumerable<Assignment>> GetAllAsync()
        {
            return await _context.Assignments.ToListAsync();
        }

        public async Task AddAsync(Assignment entity)
        {
            await _context.Assignments.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Assignment entity)
        {
            _context.Assignments.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Assignments.FindAsync(id);
            if (entity != null)
            {
                _context.Assignments.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Assignment>> FindAsync(Expression<Func<Assignment, bool>> predicate)
        {
            return await _context.Assignments.Where(predicate).ToListAsync();
        }

        public async Task<List<Assignment>> GetAllWithQuestionsAsync()
        {
            return await _context.Assignments.Include(a => a.Questions).OrderByDescending(a => a.CreatedAt).ToListAsync();
        }

        public async Task<Assignment?> GetWithQuestionsAsync(int id)
        {
            return await _context.Assignments.Include(a => a.Questions).FirstOrDefaultAsync(a => a.Id == id);
        }
    }
} 
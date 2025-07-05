using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ArnaBlazorTest.Models;
using ArnaBlazorTest.Data;
using System.Linq.Expressions;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ArnaBlazorTest.Services
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetWithRoleAndProgressAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
    }

    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task AddAsync(User entity)
        {
            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User entity)
        {
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Users.FindAsync(id);
            if (entity != null)
            {
                _context.Users.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
        {
            return await _context.Users.Where(predicate).ToListAsync();
        }

        public async Task<User?> GetWithRoleAndProgressAsync(int id)
        {
            return await _context.Users
                .Include(u => u.UserRole)
                .Include(u => u.AssignmentProgresses)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            _logger.LogInformation("Looking up user by username: {Username}", username);
            
            var user = await _context.Users
                .Include(u => u.UserRole)
                .FirstOrDefaultAsync(u => u.Username == username);
                
            _logger.LogInformation("User lookup result for {Username}: {UserFound}", username, user != null ? "Found" : "Not Found");
            
            return user;
        }
    }
} 
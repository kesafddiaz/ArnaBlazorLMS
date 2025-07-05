using ArnaBlazorTest.Data;
using ArnaBlazorTest.Models;
using Microsoft.EntityFrameworkCore;

namespace ArnaBlazorTest.Services;

public interface IUserManagementService
{
    Task<List<User>> GetAllLearnersAsync();
    Task<List<User>> GetUnassignedLearnersAsync();
    Task<List<User>> GetLearnersByManagerAsync(int managerId);
    Task<bool> AssignLearnerToManagerAsync(int learnerId, int managerId);
    Task<bool> RemoveLearnerFromManagerAsync(int learnerId);
    Task<List<User>> GetAllManagersAsync();
}

public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserManagementService> _logger;

    public UserManagementService(IUserRepository userRepository, ILogger<UserManagementService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<List<User>> GetAllLearnersAsync()
    {
        var users = await _userRepository.FindAsync(u => u.UserRole.Name == "Learner");
        return users.ToList();
    }

    public async Task<List<User>> GetUnassignedLearnersAsync()
    {
        var users = await _userRepository.FindAsync(u => u.UserRole.Name == "Learner" && u.ManagerId == null);
        return users.ToList();
    }

    public async Task<List<User>> GetLearnersByManagerAsync(int managerId)
    {
        var users = await _userRepository.FindAsync(u => u.ManagerId == managerId);
        return users.ToList();
    }

    public async Task<bool> AssignLearnerToManagerAsync(int learnerId, int managerId)
    {
        try
        {
            var learner = await _userRepository.GetByIdAsync(learnerId);
            if (learner == null)
            {
                _logger.LogWarning("Learner with ID {LearnerId} not found", learnerId);
                return false;
            }

            var manager = await _userRepository.GetByIdAsync(managerId);
            if (manager == null)
            {
                _logger.LogWarning("Manager with ID {ManagerId} not found", managerId);
                return false;
            }

            learner.ManagerId = managerId;
            await _userRepository.UpdateAsync(learner);
            
            _logger.LogInformation("Learner {LearnerId} assigned to manager {ManagerId}", learnerId, managerId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning learner {LearnerId} to manager {ManagerId}", learnerId, managerId);
            return false;
        }
    }

    public async Task<bool> RemoveLearnerFromManagerAsync(int learnerId)
    {
        try
        {
            var learner = await _userRepository.GetByIdAsync(learnerId);
            if (learner == null)
            {
                _logger.LogWarning("Learner with ID {LearnerId} not found", learnerId);
                return false;
            }

            learner.ManagerId = null;
            await _userRepository.UpdateAsync(learner);
            
            _logger.LogInformation("Learner {LearnerId} removed from manager", learnerId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing learner {LearnerId} from manager", learnerId);
            return false;
        }
    }

    public async Task<List<User>> GetAllManagersAsync()
    {
        var users = await _userRepository.FindAsync(u => u.UserRole.Name == "Manager");
        return users.ToList();
    }
} 
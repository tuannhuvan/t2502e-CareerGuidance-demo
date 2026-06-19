using CareerGuidance.Models;

namespace CareerGuidance.Services;

public interface IGoalService
{
    Task<Goal> CreateGoalAsync(Goal goal);
    Task<Goal?> GetGoalByIdAsync(int goalId);
    Task<IEnumerable<Goal>> GetStudentGoalsAsync(string studentId);
    Task UpdateGoalAsync(Goal goal);
    Task DeleteGoalAsync(int goalId);
    Task<IEnumerable<Goal>> GetActiveGoalsAsync(string studentId);
}

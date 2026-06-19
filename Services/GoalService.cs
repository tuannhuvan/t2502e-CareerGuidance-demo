using CareerGuidance.Models;
using CareerGuidance.Repositories;

namespace CareerGuidance.Services;

public class GoalService : IGoalService
{
    private readonly IUnitOfWork _unitOfWork;

    public GoalService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Goal> CreateGoalAsync(Goal goal)
    {
        goal.CreatedAt = DateTime.UtcNow;
        goal.Status = 1;
        await _unitOfWork.Goals.AddAsync(goal);
        return goal;
    }

    public async Task<Goal?> GetGoalByIdAsync(int goalId)
    {
        return await _unitOfWork.Goals.GetByIdAsync(goalId);
    }

    public async Task<IEnumerable<Goal>> GetStudentGoalsAsync(string studentId)
    {
        return await _unitOfWork.Goals.FindAsync(g => g.StudentId == studentId);
    }

    public async Task UpdateGoalAsync(Goal goal)
    {
        goal.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Goals.Update(goal);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteGoalAsync(int goalId)
    {
        var goal = await _unitOfWork.Goals.GetByIdAsync(goalId);
        if (goal != null)
        {
            goal.Status = 0;
            _unitOfWork.Goals.Update(goal);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Goal>> GetActiveGoalsAsync(string studentId)
    {
        return await _unitOfWork.Goals.FindAsync(g => g.StudentId == studentId && g.Status == 1);
    }
}

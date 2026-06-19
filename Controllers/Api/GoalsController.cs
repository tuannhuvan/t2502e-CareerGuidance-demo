using CareerGuidance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerGuidance.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Student")]
public class GoalsController : ControllerBase
{
    private readonly IGoalService _goalService;
    private readonly ILogger<GoalsController> _logger;

    public GoalsController(IGoalService goalService, ILogger<GoalsController> logger)
    {
        _goalService = goalService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyGoals()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(new { success = false, message = "User not found" });

            var goals = await _goalService.GetStudentGoalsAsync(userId);
            return Ok(new { success = true, data = goals });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving goals");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateGoal([FromBody] CreateGoalRequest request)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(new { success = false, message = "User not found" });

            var goal = new CareerGuidance.Models.Goal
            {
                StudentId = userId,
                CareerPathId = request.CareerPathId,
                Title = request.Title,
                Description = request.Description,
                TargetDate = request.TargetDate
            };

            var createdGoal = await _goalService.CreateGoalAsync(goal);
            return Ok(new { success = true, data = createdGoal });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating goal");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}

public class CreateGoalRequest
{
    public int CareerPathId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime TargetDate { get; set; }
}

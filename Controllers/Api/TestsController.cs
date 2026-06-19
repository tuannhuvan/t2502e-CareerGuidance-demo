using CareerGuidance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerGuidance.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class TestsController : ControllerBase
{
    private readonly ITestService _testService;
    private readonly ILogger<TestsController> _logger;

    public TestsController(ITestService testService, ILogger<TestsController> logger)
    {
        _testService = testService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetActiveTests()
    {
        try
        {
            var tests = await _testService.GetActiveTestsAsync();
            return Ok(new { success = true, data = tests });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active tests");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTest(int id)
    {
        try
        {
            var test = await _testService.GetTestByIdAsync(id);
            if (test == null)
                return NotFound(new { success = false, message = "Test not found" });

            return Ok(new { success = true, data = test });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving test {TestId}", id);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{id}/questions")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTestQuestions(int id)
    {
        try
        {
            var questions = await _testService.GetTestQuestionsAsync(id);
            return Ok(new { success = true, data = questions });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving questions for test {TestId}", id);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpPost("start")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> StartTest([FromBody] StartTestRequest request)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(new { success = false, message = "User not found" });

            var testResult = await _testService.CreateTestResultAsync(request.TestId, userId);
            return Ok(new { success = true, data = testResult });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting test");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpPost("{resultId}/answer")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> SaveAnswer(int resultId, [FromBody] SaveAnswerRequest request)
    {
        try
        {
            await _testService.SaveTestAnswerAsync(resultId, request.QuestionId, request.OptionId);
            return Ok(new { success = true, message = "Answer saved" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving answer");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpPost("{resultId}/complete")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CompleteTest(int resultId)
    {
        try
        {
            var result = await _testService.CompleteTestAsync(resultId);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing test");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}

public class StartTestRequest
{
    public int TestId { get; set; }
}

public class SaveAnswerRequest
{
    public int QuestionId { get; set; }
    public int OptionId { get; set; }
}

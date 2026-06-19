using CareerGuidance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerGuidance.Controllers;

[Authorize(Roles = "Student")]
public class TestsController : Controller
{
    private readonly ITestService _testService;
    private readonly ILogger<TestsController> _logger;

    public TestsController(ITestService testService, ILogger<TestsController> logger)
    {
        _testService = testService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var tests = await _testService.GetActiveTestsAsync();
            return View(tests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active tests");
            return View(new List<CareerGuidance.Models.AssessmentTest>());
        }
    }

    public async Task<IActionResult> Take(int testId)
    {
        try
        {
            var test = await _testService.GetTestByIdAsync(testId);
            if (test == null)
                return NotFound();

            return View(test);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading test {TestId}", testId);
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> MyResults()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var results = await _testService.GetStudentTestResultsAsync(userId);
            return View(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving test results");
            return View(new List<CareerGuidance.Models.TestResult>());
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var result = await _testService.GetTestResultAsync(id);
            if (result == null)
                return NotFound();

            // Verify ownership
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (result.StudentId != userId)
                return Forbid();

            return View(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving test result {ResultId}", id);
            return NotFound();
        }
    }
}

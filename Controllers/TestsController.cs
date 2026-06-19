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

    public IActionResult Results(int id)
    {
        ViewBag.ResultId = id;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetQuestions(int testId)
    {
        var questions = await _testService.GetTestQuestionsAsync(testId);
        var payload = questions.Select(q => new
        {
            id = q.Id,
            content = q.Content,
            type = q.QuestionType?.Name,
            options = q.Options.Select(o => new { id = o.Id, content = o.Content })
        });
        return Json(payload);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit([FromBody] SubmitTestRequest request)
    {
        if (request == null || request.Answers == null || request.Answers.Count == 0)
        {
            return BadRequest(new { success = false, message = "Vui lòng trả lời ít nhất một câu hỏi." });
        }

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        try
        {
            var answers = request.Answers
                .Select(a => new AnswerSubmission(a.QuestionId, a.OptionId))
                .ToList();
            var result = await _testService.SubmitTestAsync(userId, request.TestId, answers);
            return Json(new { success = true, resultId = result.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting test {TestId}", request.TestId);
            return StatusCode(500, new { success = false, message = "Không thể chấm điểm bài test." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetResult(int id)
    {
        var result = await _testService.GetTestResultAsync(id);
        if (result == null)
            return NotFound();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (result.StudentId != userId)
            return Forbid();

        return Json(new
        {
            testTitle = result.Test?.Title,
            score = result.Score,
            compatibilityScore = result.CompatibilityScore,
            recommendedCareerPathId = result.RecommendedCareerPathId,
            recommendedCareerPath = result.RecommendedCareerPath?.Title,
            answerCount = result.TestAnswers.Count
        });
    }
}

public class SubmitTestRequest
{
    public int TestId { get; set; }
    public List<SubmitAnswer> Answers { get; set; } = new();
}

public class SubmitAnswer
{
    public int QuestionId { get; set; }
    public int OptionId { get; set; }
}

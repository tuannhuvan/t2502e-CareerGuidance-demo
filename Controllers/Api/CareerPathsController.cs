using CareerGuidance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerGuidance.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class CareerPathsController : ControllerBase
{
    private readonly ICareerPathService _careerPathService;
    private readonly ILogger<CareerPathsController> _logger;

    public CareerPathsController(ICareerPathService careerPathService, ILogger<CareerPathsController> logger)
    {
        _careerPathService = careerPathService;
        _logger = logger;
    }

    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var categories = await _careerPathService.GetAllCategoriesAsync();
            return Ok(new { success = true, data = categories });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpGet("category/{categoryId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCareerPathsByCategory(int categoryId)
    {
        try
        {
            var paths = await _careerPathService.GetCareerPathsByCategoryAsync(categoryId);
            return Ok(new { success = true, data = paths });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving career paths for category {CategoryId}", categoryId);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCareerPath(int id)
    {
        try
        {
            var path = await _careerPathService.GetCareerPathByIdAsync(id);
            if (path == null)
                return NotFound(new { success = false, message = "Career path not found" });

            return Ok(new { success = true, data = path });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving career path {Id}", id);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{id}/resources")]
    [AllowAnonymous]
    public async Task<IActionResult> GetResources(int id)
    {
        try
        {
            var resources = await _careerPathService.GetResourcesByCareerPathAsync(id);
            return Ok(new { success = true, data = resources });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving resources for career path {Id}", id);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{id}/jobs")]
    [AllowAnonymous]
    public async Task<IActionResult> GetJobs(int id)
    {
        try
        {
            var jobs = await _careerPathService.GetJobsByCareerPathAsync(id);
            return Ok(new { success = true, data = jobs });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving jobs for career path {Id}", id);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpGet("search/{keyword}")]
    [AllowAnonymous]
    public async Task<IActionResult> Search(string keyword)
    {
        try
        {
            var results = await _careerPathService.SearchCareerPathsAsync(keyword);
            return Ok(new { success = true, data = results });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching career paths");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}

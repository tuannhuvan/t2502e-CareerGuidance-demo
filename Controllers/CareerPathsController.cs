using CareerGuidance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerGuidance.Controllers;

[AllowAnonymous]
public class CareerPathsController : Controller
{
    private readonly ICareerPathService _careerPathService;
    private readonly ILogger<CareerPathsController> _logger;

    public CareerPathsController(ICareerPathService careerPathService, ILogger<CareerPathsController> logger)
    {
        _careerPathService = careerPathService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var categories = await _careerPathService.GetAllCategoriesAsync();
            // Get all active career paths
            var allPaths = new List<CareerGuidance.Models.CareerPath>();
            foreach (var category in categories)
            {
                var paths = await _careerPathService.GetCareerPathsByCategoryAsync(category.Id);
                allPaths.AddRange(paths);
            }
            return View(allPaths);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving career paths");
            return View(new List<CareerGuidance.Models.CareerPath>());
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var careerPath = await _careerPathService.GetCareerPathByIdAsync(id);
            if (careerPath == null)
                return NotFound();

            return View(careerPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving career path {Id}", id);
            return NotFound();
        }
    }

    public async Task<IActionResult> Search(string keyword)
    {
        try
        {
            if (string.IsNullOrEmpty(keyword))
                return RedirectToAction(nameof(Index));

            var results = await _careerPathService.SearchCareerPathsAsync(keyword);
            ViewBag.SearchKeyword = keyword;
            return View(nameof(Index), results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching career paths");
            return View(nameof(Index), new List<CareerGuidance.Models.CareerPath>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetDetails(int id)
    {
        var careerPath = await _careerPathService.GetCareerPathByIdAsync(id);
        if (careerPath == null)
            return NotFound();

        var resources = await _careerPathService.GetResourcesAsync(id);
        var jobs = await _careerPathService.GetJobPostingsAsync(id);

        return Json(new
        {
            category = careerPath.Category?.Name,
            resources = resources.Select(r => new { r.Title, r.ResourceType, r.Url }),
            jobs = jobs.Select(j => new { j.Title, j.CompanyName, j.Salary, j.Description })
        });
    }
}

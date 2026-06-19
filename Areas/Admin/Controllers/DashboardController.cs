using CareerGuidance.Models;
using CareerGuidance.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerGuidance.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IUnitOfWork unitOfWork, ILogger<DashboardController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var totalTests = await _unitOfWork.Tests.CountAsync();
            var totalCareerPaths = await _unitOfWork.CareerPaths.CountAsync();
            var totalResults = await _unitOfWork.TestResults.CountAsync();
            var totalCategories = await _unitOfWork.Categories.CountAsync();

            ViewBag.TotalTests = totalTests;
            ViewBag.TotalCareerPaths = totalCareerPaths;
            ViewBag.TotalResults = totalResults;
            ViewBag.TotalCategories = totalCategories;

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            return View();
        }
    }
}

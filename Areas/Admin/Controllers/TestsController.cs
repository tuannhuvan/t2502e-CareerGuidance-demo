using CareerGuidance.Models;
using CareerGuidance.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerGuidance.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class TestsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TestsController> _logger;

    public TestsController(IUnitOfWork unitOfWork, ILogger<TestsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var tests = await _unitOfWork.Tests.GetAllAsync();
            return View(tests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tests");
            return View(new List<AssessmentTest>());
        }
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AssessmentTest test)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(test);

            test.CreatedAt = DateTime.UtcNow;
            test.Status = 1;
            await _unitOfWork.Tests.AddAsync(test);

            return RedirectToAction(nameof(Index)).WithSuccess("Test created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating test");
            ModelState.AddModelError(string.Empty, "Error creating test");
            return View(test);
        }
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (!id.HasValue)
            return NotFound();

        var test = await _unitOfWork.Tests.GetByIdAsync(id.Value);
        if (test == null)
            return NotFound();

        return View(test);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AssessmentTest test)
    {
        if (id != test.Id)
            return NotFound();

        try
        {
            if (!ModelState.IsValid)
                return View(test);

            test.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Tests.Update(test);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index)).WithSuccess("Test updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating test");
            ModelState.AddModelError(string.Empty, "Error updating test");
            return View(test);
        }
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (!id.HasValue)
            return NotFound();

        var test = await _unitOfWork.Tests.GetByIdAsync(id.Value);
        if (test == null)
            return NotFound();

        return View(test);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var test = await _unitOfWork.Tests.GetByIdAsync(id);
            if (test != null)
            {
                test.Status = 0;
                _unitOfWork.Tests.Update(test);
                await _unitOfWork.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index)).WithSuccess("Test deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting test");
            return RedirectToAction(nameof(Index)).WithError("Error deleting test");
        }
    }
}

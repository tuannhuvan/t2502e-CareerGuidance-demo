using CareerGuidance.Models;
using CareerGuidance.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerGuidance.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(IUnitOfWork unitOfWork, ILogger<CategoriesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return View(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading categories");
            return View(new List<Category>());
        }
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(category);

            category.CreatedAt = DateTime.UtcNow;
            category.Status = 1;
            await _unitOfWork.Categories.AddAsync(category);

            return RedirectToAction(nameof(Index)).WithSuccess("Category created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            ModelState.AddModelError(string.Empty, "Error creating category");
            return View(category);
        }
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (!id.HasValue)
            return NotFound();

        var category = await _unitOfWork.Categories.GetByIdAsync(id.Value);
        if (category == null)
            return NotFound();

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.Id)
            return NotFound();

        try
        {
            if (!ModelState.IsValid)
                return View(category);

            category.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index)).WithSuccess("Category updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category");
            ModelState.AddModelError(string.Empty, "Error updating category");
            return View(category);
        }
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (!id.HasValue)
            return NotFound();

        var category = await _unitOfWork.Categories.GetByIdAsync(id.Value);
        if (category == null)
            return NotFound();

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category != null)
            {
                category.Status = 0;
                _unitOfWork.Categories.Update(category);
                await _unitOfWork.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index)).WithSuccess("Category deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");
            return RedirectToAction(nameof(Index)).WithError("Error deleting category");
        }
    }
}

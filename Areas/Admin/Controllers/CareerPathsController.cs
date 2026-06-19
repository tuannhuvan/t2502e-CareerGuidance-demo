using CareerGuidance.Models;
using CareerGuidance.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CareerGuidance.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CareerPathsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CareerPathsController> _logger;

    public CareerPathsController(IUnitOfWork unitOfWork, ILogger<CareerPathsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var careerPaths = await _unitOfWork.CareerPaths.GetAllAsync();
            return View(careerPaths);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading career paths");
            return View(new List<CareerPath>());
        }
    }

    public async Task<IActionResult> Create()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CareerPath careerPath)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var categories = await _unitOfWork.Categories.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", careerPath.CategoryId);
                return View(careerPath);
            }

            careerPath.CreatedAt = DateTime.UtcNow;
            careerPath.Status = 1;
            await _unitOfWork.CareerPaths.AddAsync(careerPath);

            return RedirectToAction(nameof(Index)).WithSuccess("Career path created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating career path");
            ModelState.AddModelError(string.Empty, "Error creating career path");
            return View(careerPath);
        }
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (!id.HasValue)
            return NotFound();

        var careerPath = await _unitOfWork.CareerPaths.GetByIdAsync(id.Value);
        if (careerPath == null)
            return NotFound();

        var categories = await _unitOfWork.Categories.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", careerPath.CategoryId);
        return View(careerPath);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CareerPath careerPath)
    {
        if (id != careerPath.Id)
            return NotFound();

        try
        {
            if (!ModelState.IsValid)
            {
                var categories = await _unitOfWork.Categories.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", careerPath.CategoryId);
                return View(careerPath);
            }

            careerPath.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.CareerPaths.Update(careerPath);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index)).WithSuccess("Career path updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating career path");
            ModelState.AddModelError(string.Empty, "Error updating career path");
            return View(careerPath);
        }
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (!id.HasValue)
            return NotFound();

        var careerPath = await _unitOfWork.CareerPaths.GetByIdAsync(id.Value);
        if (careerPath == null)
            return NotFound();

        return View(careerPath);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var careerPath = await _unitOfWork.CareerPaths.GetByIdAsync(id);
            if (careerPath != null)
            {
                careerPath.Status = 0;
                _unitOfWork.CareerPaths.Update(careerPath);
                await _unitOfWork.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index)).WithSuccess("Career path deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting career path");
            return RedirectToAction(nameof(Index)).WithError("Error deleting career path");
        }
    }
}

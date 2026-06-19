using CareerGuidance.Data;
using CareerGuidance.Helpers;
using CareerGuidance.Models;
using CareerGuidance.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerGuidance.Areas.Admin.Controllers;

public class CareerPathsController : BaseAdminController
{
    private readonly AppDbContext _context;
    private const int PageSize = 10;

    public CareerPathsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(CareerPathFilterViewModel filter, int? pageIndex)
    {
        var query = _context.CareerPaths
            .Include(c => c.Category)
            .Where(c => c.Status != 3);

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            query = query.Where(c =>
                c.Title.Contains(filter.Keyword) ||
                (c.Content != null && c.Content.Contains(filter.Keyword)));
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(c => c.CategoryId == filter.CategoryId.Value);
        }

        query = filter.SortOrder switch
        {
            "title_desc" => query.OrderByDescending(c => c.Title),
            "date_desc" => query.OrderByDescending(c => c.CreatedAt),
            _ => query.OrderBy(c => c.Title)
        };

        filter.CategoryOptions = await _context.Categories
            .Where(c => c.Status == 1)
            .OrderBy(c => c.Name)
            .AsNoTracking()
            .ToListAsync();

        filter.CareerPaths = await PaginatedList<CareerPath>.CreateAsync(query.AsNoTracking(), pageIndex ?? 1, PageSize);
        return View(filter);
    }

    private async Task LoadCategoryOptionsAsync(CareerPathFormViewModel model)
    {
        model.CategoryOptions = await _context.Categories
            .Where(c => c.Status == 1)
            .OrderBy(c => c.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IActionResult> Create()
    {
        var model = new CareerPathFormViewModel();
        await LoadCategoryOptionsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CareerPathFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoryOptionsAsync(model);
            return View(model);
        }

        _context.CareerPaths.Add(new CareerPath
        {
            CategoryId = model.CategoryId,
            Title = model.Title,
            Content = model.Content,
            Status = model.Status,
            CreatedBy = User.Identity?.Name ?? "Admin"
        });
        await _context.SaveChangesAsync();
        TempData["Success"] = "Thêm lộ trình nghề nghiệp thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _context.CareerPaths.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        var model = new CareerPathFormViewModel
        {
            Id = entity.Id,
            CategoryId = entity.CategoryId,
            Title = entity.Title,
            Content = entity.Content,
            Status = entity.Status
        };
        await LoadCategoryOptionsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CareerPathFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoryOptionsAsync(model);
            return View(model);
        }

        var entity = await _context.CareerPaths.FindAsync(model.Id);
        if (entity == null)
        {
            return NotFound();
        }

        entity.CategoryId = model.CategoryId;
        entity.Title = model.Title;
        entity.Content = model.Content;
        entity.Status = model.Status;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = User.Identity?.Name ?? "Admin";
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật lộ trình thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSelected(List<int> ids)
    {
        if (ids == null || ids.Count == 0)
        {
            return Json(new { success = false, message = "Không có mục nào được chọn." });
        }

        var items = await _context.CareerPaths.Where(c => ids.Contains(c.Id)).ToListAsync();
        foreach (var item in items)
        {
            item.Status = 3;
            item.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return Json(new { success = true, message = $"Đã xóa mềm {items.Count} lộ trình." });
    }
}

using CareerGuidance.Data;
using CareerGuidance.Helpers;
using CareerGuidance.Models;
using CareerGuidance.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerGuidance.Areas.Admin.Controllers;

public class CategoriesController : BaseAdminController
{
    private readonly AppDbContext _context;
    private const int PageSize = 10;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(CategoryFilterViewModel filter, int? pageIndex)
    {
        var query = _context.Categories.Where(c => c.Status != 3);

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            query = query.Where(c =>
                c.Name.Contains(filter.Keyword) ||
                (c.Description != null && c.Description.Contains(filter.Keyword)));
        }

        query = QueryFilterHelper.ApplyDateRange(query, filter.DateRange);

        query = filter.SortOrder switch
        {
            "name_desc" => query.OrderByDescending(c => c.Name),
            "date_desc" => query.OrderByDescending(c => c.CreatedAt),
            _ => query.OrderBy(c => c.Name)
        };

        filter.Categories = await PaginatedList<Category>.CreateAsync(query.AsNoTracking(), pageIndex ?? 1, PageSize);
        return View(filter);
    }

    public IActionResult Create() => View(new CategoryFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _context.Categories.Add(new Category
        {
            Name = model.Name,
            Description = model.Description,
            Status = model.Status,
            CreatedBy = User.Identity?.Name ?? "Admin"
        });
        await _context.SaveChangesAsync();
        TempData["Success"] = "Thêm danh mục thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _context.Categories.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        return View(new CategoryFormViewModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Status = entity.Status
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var entity = await _context.Categories.FindAsync(model.Id);
        if (entity == null)
        {
            return NotFound();
        }

        entity.Name = model.Name;
        entity.Description = model.Description;
        entity.Status = model.Status;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = User.Identity?.Name ?? "Admin";
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật danh mục thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSelected(List<int> ids)
    {
        if (ids == null || ids.Count == 0)
        {
            return Json(new { success = false, message = "Không có mục nào được chọn." });
        }

        var items = await _context.Categories.Where(c => ids.Contains(c.Id)).ToListAsync();
        foreach (var item in items)
        {
            item.Status = 3;
            item.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return Json(new { success = true, message = $"Đã xóa mềm {items.Count} danh mục." });
    }
}

using CareerGuidance.Data;
using CareerGuidance.Helpers;
using CareerGuidance.Models;
using CareerGuidance.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerGuidance.Areas.Admin.Controllers;

public class TestsController : BaseAdminController
{
    private readonly AppDbContext _context;
    private const int PageSize = 10;

    public TestsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(TestFilterViewModel filter, int? pageIndex)
    {
        var query = _context.Tests.Where(t => t.Status != 3);

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            query = query.Where(t =>
                t.Title.Contains(filter.Keyword) ||
                (t.Description != null && t.Description.Contains(filter.Keyword)));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(t => t.Status == filter.Status.Value);
        }

        query = QueryFilterHelper.ApplyDateRange(query, filter.DateRange);

        query = filter.SortOrder switch
        {
            "title_desc" => query.OrderByDescending(t => t.Title),
            "date_asc" => query.OrderBy(t => t.CreatedAt),
            "date_desc" => query.OrderByDescending(t => t.CreatedAt),
            _ => query.OrderBy(t => t.Title)
        };

        filter.Tests = await PaginatedList<AssessmentTest>.CreateAsync(query.AsNoTracking(), pageIndex ?? 1, PageSize);
        return View(filter);
    }

    public IActionResult Create()
    {
        return View(new TestFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TestFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var entity = new AssessmentTest
        {
            Title = model.Title,
            Description = model.Description,
            Status = model.Status,
            CreatedBy = User.Identity?.Name ?? "Admin"
        };

        _context.Tests.Add(entity);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Thêm bài test thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _context.Tests.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        return View(new TestFormViewModel
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Status = entity.Status
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TestFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var entity = await _context.Tests.FindAsync(model.Id);
        if (entity == null)
        {
            return NotFound();
        }

        entity.Title = model.Title;
        entity.Description = model.Description;
        entity.Status = model.Status;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = User.Identity?.Name ?? "Admin";

        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật bài test thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.Tests.FindAsync(id);
        if (entity != null)
        {
            entity.Status = 3;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSelected(List<int> ids)
    {
        if (ids == null || ids.Count == 0)
        {
            return Json(new { success = false, message = "Không có mục nào được chọn." });
        }

        var items = await _context.Tests.Where(t => ids.Contains(t.Id)).ToListAsync();
        foreach (var item in items)
        {
            item.Status = 3;
            item.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return Json(new { success = true, message = $"Đã xóa mềm {items.Count} bài test." });
    }
}

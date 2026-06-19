using CareerGuidance.Data;
using CareerGuidance.Models;
using Microsoft.EntityFrameworkCore;

namespace CareerGuidance.Services;

public class CareerPathService : ICareerPathService
{
    private readonly AppDbContext _context;

    public CareerPathService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories
            .Where(c => c.Status == 1)
            .OrderBy(c => c.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<CareerPath>> GetCareerPathsByCategoryAsync(int categoryId)
    {
        return await _context.CareerPaths
            .Include(c => c.Category)
            .Where(c => c.Status == 1 && c.CategoryId == categoryId)
            .OrderBy(c => c.Title)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<CareerPath?> GetCareerPathByIdAsync(int id)
    {
        return await _context.CareerPaths
            .Include(c => c.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.Status == 1);
    }

    public async Task<IReadOnlyList<CareerPath>> SearchCareerPathsAsync(string keyword)
    {
        return await _context.CareerPaths
            .Include(c => c.Category)
            .Where(c => c.Status == 1 &&
                (c.Title.Contains(keyword) ||
                 (c.Content != null && c.Content.Contains(keyword))))
            .OrderBy(c => c.Title)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Resource>> GetResourcesAsync(int careerPathId)
    {
        return await _context.Resources
            .Where(r => r.PathId == careerPathId && r.Status == 1)
            .OrderBy(r => r.Title)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<JobPosting>> GetJobPostingsAsync(int careerPathId)
    {
        return await _context.JobPostings
            .Where(j => j.CareerPathId == careerPathId && j.Status == 1)
            .OrderByDescending(j => j.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }
}

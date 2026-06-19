using CareerGuidance.Models;
using CareerGuidance.Repositories;

namespace CareerGuidance.Services;

public class CareerPathService : ICareerPathService
{
    private readonly IUnitOfWork _unitOfWork;

    public CareerPathService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _unitOfWork.Categories.GetAllAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int categoryId)
    {
        return await _unitOfWork.Categories.GetByIdAsync(categoryId);
    }

    public async Task<IEnumerable<CareerPath>> GetCareerPathsByCategoryAsync(int categoryId)
    {
        return await _unitOfWork.CareerPaths.FindAsync(c => c.CategoryId == categoryId && c.Status == 1);
    }

    public async Task<CareerPath?> GetCareerPathByIdAsync(int careerPathId)
    {
        return await _unitOfWork.CareerPaths.GetByIdAsync(careerPathId);
    }

    public async Task<IEnumerable<Resource>> GetResourcesByCareerPathAsync(int careerPathId)
    {
        return await _unitOfWork.Resources.FindAsync(r => r.PathId == careerPathId && r.Status == 1);
    }

    public async Task<IEnumerable<JobPosting>> GetJobsByCareerPathAsync(int careerPathId)
    {
        return await _unitOfWork.JobPostings.FindAsync(j => j.CareerPathId == careerPathId && j.Status == 1);
    }

    public async Task<CareerPath> CreateCareerPathAsync(CareerPath careerPath)
    {
        careerPath.CreatedAt = DateTime.UtcNow;
        careerPath.Status = 1;
        await _unitOfWork.CareerPaths.AddAsync(careerPath);
        return careerPath;
    }

    public async Task UpdateCareerPathAsync(CareerPath careerPath)
    {
        careerPath.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.CareerPaths.Update(careerPath);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteCareerPathAsync(int careerPathId)
    {
        var careerPath = await _unitOfWork.CareerPaths.GetByIdAsync(careerPathId);
        if (careerPath != null)
        {
            careerPath.Status = 0;
            _unitOfWork.CareerPaths.Update(careerPath);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<CareerPath>> SearchCareerPathsAsync(string keyword)
    {
        return await _unitOfWork.CareerPaths.FindAsync(c =>
            c.Title.Contains(keyword) && c.Status == 1);
    }
}

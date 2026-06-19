using CareerGuidance.Models;

namespace CareerGuidance.Services;

public interface ICareerPathService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int categoryId);
    Task<IEnumerable<CareerPath>> GetCareerPathsByCategoryAsync(int categoryId);
    Task<CareerPath?> GetCareerPathByIdAsync(int careerPathId);
    Task<IEnumerable<Resource>> GetResourcesByCareerPathAsync(int careerPathId);
    Task<IEnumerable<JobPosting>> GetJobsByCareerPathAsync(int careerPathId);
    Task<CareerPath> CreateCareerPathAsync(CareerPath careerPath);
    Task UpdateCareerPathAsync(CareerPath careerPath);
    Task DeleteCareerPathAsync(int careerPathId);
    Task<IEnumerable<CareerPath>> SearchCareerPathsAsync(string keyword);
}

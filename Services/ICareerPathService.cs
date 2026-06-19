using CareerGuidance.Models;

namespace CareerGuidance.Services;

public interface ICareerPathService
{
    Task<IReadOnlyList<Category>> GetAllCategoriesAsync();
    Task<IReadOnlyList<CareerPath>> GetCareerPathsByCategoryAsync(int categoryId);
    Task<CareerPath?> GetCareerPathByIdAsync(int id);
    Task<IReadOnlyList<CareerPath>> SearchCareerPathsAsync(string keyword);
    Task<IReadOnlyList<Resource>> GetResourcesAsync(int careerPathId);
    Task<IReadOnlyList<JobPosting>> GetJobPostingsAsync(int careerPathId);
}

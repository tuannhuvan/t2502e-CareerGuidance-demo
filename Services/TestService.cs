using CareerGuidance.Data;
using CareerGuidance.Models;
using Microsoft.EntityFrameworkCore;

namespace CareerGuidance.Services;

public class TestService : ITestService
{
    private readonly AppDbContext _context;

    public TestService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AssessmentTest>> GetActiveTestsAsync()
    {
        return await _context.Tests
            .Where(t => t.Status == 1)
            .OrderBy(t => t.Title)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<AssessmentTest?> GetTestByIdAsync(int testId)
    {
        return await _context.Tests
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == testId && t.Status == 1);
    }

    public async Task<IReadOnlyList<QuestionTest>> GetTestQuestionsAsync(int testId)
    {
        return await _context.QuestionTests
            .Include(q => q.QuestionType)
            .Include(q => q.Options.Where(o => o.Status == 1))
            .Where(q => q.TestId == testId && q.Status == 1)
            .OrderBy(q => q.Id)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<TestResult>> GetStudentTestResultsAsync(string userId)
    {
        return await _context.TestResults
            .Include(r => r.Test)
            .Include(r => r.RecommendedCareerPath)
            .Where(r => r.StudentId == userId)
            .OrderByDescending(r => r.CompletedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TestResult?> GetTestResultAsync(int id)
    {
        return await _context.TestResults
            .Include(r => r.Test)
            .Include(r => r.RecommendedCareerPath)
            .Include(r => r.TestAnswers)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<TestResult> SubmitTestAsync(string userId, int testId, IReadOnlyList<AnswerSubmission> answers)
    {
        var test = await _context.Tests.FirstOrDefaultAsync(t => t.Id == testId && t.Status == 1)
            ?? throw new InvalidOperationException($"Test {testId} not found or inactive.");

        var selectedOptionIds = answers.Select(a => a.OptionId).Distinct().ToList();

        // Aggregate career-path weights for the selected options.
        var weights = await _context.OptionCareerPaths
            .Where(o => selectedOptionIds.Contains(o.OptionId))
            .GroupBy(o => o.CareerPathId)
            .Select(g => new { CareerPathId = g.Key, TotalWeight = g.Sum(x => x.Weight) })
            .ToListAsync();

        int? recommendedCareerPathId = null;
        int totalScore = 0;
        decimal? compatibilityScore = null;

        if (weights.Count > 0)
        {
            var best = weights.OrderByDescending(w => w.TotalWeight).First();
            recommendedCareerPathId = best.CareerPathId;
            totalScore = best.TotalWeight;

            var allWeight = weights.Sum(w => w.TotalWeight);
            if (allWeight > 0)
            {
                compatibilityScore = Math.Round(best.TotalWeight * 100m / allWeight, 2);
            }
        }

        var result = new TestResult
        {
            StudentId = userId,
            TestId = testId,
            RecommendedCareerPathId = recommendedCareerPathId,
            Score = totalScore,
            CompatibilityScore = compatibilityScore,
            CompletedAt = DateTime.UtcNow,
            CreatedBy = userId,
            TestAnswers = answers
                .Select(a => new TestAnswer { QuestionId = a.QuestionId, OptionId = a.OptionId, CreatedBy = userId })
                .ToList()
        };

        _context.TestResults.Add(result);
        await _context.SaveChangesAsync();

        return result;
    }
}

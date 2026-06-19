using CareerGuidance.Models;
using CareerGuidance.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CareerGuidance.Services;

public class TestService : ITestService
{
    private readonly IUnitOfWork _unitOfWork;

    public TestService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AssessmentTest?> GetTestByIdAsync(int testId)
    {
        return await _unitOfWork.Tests.GetByIdAsync(testId);
    }

    public async Task<IEnumerable<AssessmentTest>> GetAllTestsAsync()
    {
        return await _unitOfWork.Tests.GetAllAsync();
    }

    public async Task<IEnumerable<QuestionTest>> GetTestQuestionsAsync(int testId)
    {
        return await _unitOfWork.Questions.FindAsync(q => q.TestId == testId);
    }

    public async Task<IEnumerable<QuestionOption>> GetQuestionOptionsAsync(int questionId)
    {
        return await _unitOfWork.QuestionOptions.FindAsync(o => o.QuestionId == questionId);
    }

    public async Task<TestResult> CreateTestResultAsync(int testId, string studentId)
    {
        var testResult = new TestResult
        {
            TestId = testId,
            StudentId = studentId,
            StartedAt = DateTime.UtcNow,
            Status = 1
        };

        await _unitOfWork.TestResults.AddAsync(testResult);
        return testResult;
    }

    public async Task<TestResult?> GetTestResultAsync(int resultId)
    {
        return await _unitOfWork.TestResults.GetByIdAsync(resultId);
    }

    public async Task SaveTestAnswerAsync(int resultId, int questionId, int optionId)
    {
        var answer = new TestAnswer
        {
            ResultId = resultId,
            QuestionId = questionId,
            OptionId = optionId
        };

        await _unitOfWork.TestAnswers.AddAsync(answer);
    }

    public async Task<TestResult> CompleteTestAsync(int resultId)
    {
        var testResult = await _unitOfWork.TestResults.GetByIdAsync(resultId);
        if (testResult == null)
            throw new InvalidOperationException($"Test result {resultId} not found");

        testResult.CompletedAt = DateTime.UtcNow;
        testResult.Status = 2; // Completed

        // Calculate recommended career path based on answers
        var answers = await _unitOfWork.TestAnswers.FindAsync(a => a.ResultId == resultId);
        var careerPathScores = new Dictionary<int, int>();

        foreach (var answer in answers)
        {
            var optionCareerPaths = await _unitOfWork.OptionCareerPaths.FindAsync(
                o => o.OptionId == answer.OptionId);

            foreach (var ocp in optionCareerPaths)
            {
                if (!careerPathScores.ContainsKey(ocp.CareerPathId))
                    careerPathScores[ocp.CareerPathId] = 0;

                careerPathScores[ocp.CareerPathId] += ocp.Weight;
            }
        }

        if (careerPathScores.Any())
        {
            var topCareerPath = careerPathScores.OrderByDescending(x => x.Value).First();
            testResult.RecommendedCareerPathId = topCareerPath.Key;
            testResult.Score = topCareerPath.Value;
        }

        _unitOfWork.TestResults.Update(testResult);
        await _unitOfWork.SaveChangesAsync();

        return testResult;
    }

    public async Task<IEnumerable<TestResult>> GetStudentTestResultsAsync(string studentId)
    {
        return await _unitOfWork.TestResults.FindAsync(r => r.StudentId == studentId);
    }

    public async Task<IEnumerable<AssessmentTest>> GetActiveTestsAsync()
    {
        return await _unitOfWork.Tests.FindAsync(t => t.Status == 1);
    }
}

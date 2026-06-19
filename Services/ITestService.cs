using CareerGuidance.Models;
using CareerGuidance.ViewModels;

namespace CareerGuidance.Services;

public interface ITestService
{
    Task<AssessmentTest?> GetTestByIdAsync(int testId);
    Task<IEnumerable<AssessmentTest>> GetAllTestsAsync();
    Task<IEnumerable<QuestionTest>> GetTestQuestionsAsync(int testId);
    Task<IEnumerable<QuestionOption>> GetQuestionOptionsAsync(int questionId);
    Task<TestResult> CreateTestResultAsync(int testId, string studentId);
    Task<TestResult?> GetTestResultAsync(int resultId);
    Task SaveTestAnswerAsync(int resultId, int questionId, int optionId);
    Task<TestResult> CompleteTestAsync(int resultId);
    Task<IEnumerable<TestResult>> GetStudentTestResultsAsync(string studentId);
    Task<IEnumerable<AssessmentTest>> GetActiveTestsAsync();
}

using CareerGuidance.Models;

namespace CareerGuidance.Services;

public record AnswerSubmission(int QuestionId, int OptionId);

public interface ITestService
{
    Task<IReadOnlyList<AssessmentTest>> GetActiveTestsAsync();
    Task<AssessmentTest?> GetTestByIdAsync(int testId);
    Task<IReadOnlyList<QuestionTest>> GetTestQuestionsAsync(int testId);
    Task<IReadOnlyList<TestResult>> GetStudentTestResultsAsync(string userId);
    Task<TestResult?> GetTestResultAsync(int id);
    Task<TestResult> SubmitTestAsync(string userId, int testId, IReadOnlyList<AnswerSubmission> answers);
}

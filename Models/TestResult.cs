namespace CareerGuidance.Models;

public class TestResult : BaseEntity
{
    public int Id { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public int TestId { get; set; }
    public int? RecommendedCareerPathId { get; set; }
    public decimal? CompatibilityScore { get; set; }

    public AppUser Student { get; set; } = null!;
    public AssessmentTest Test { get; set; } = null!;
    public CareerPath? RecommendedCareerPath { get; set; }
    public ICollection<TestAnswer> TestAnswers { get; set; } = [];
}

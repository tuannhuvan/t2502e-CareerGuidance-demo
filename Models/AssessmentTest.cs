using System.ComponentModel.DataAnnotations;

namespace CareerGuidance.Models;

public class AssessmentTest : BaseEntity
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tiêu đề bài test không được để trống")]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<QuestionTest> Questions { get; set; } = [];
    public ICollection<TestResult> TestResults { get; set; } = [];
}

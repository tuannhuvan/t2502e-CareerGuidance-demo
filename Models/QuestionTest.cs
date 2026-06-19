using System.ComponentModel.DataAnnotations;

namespace CareerGuidance.Models;

public class QuestionTest : BaseEntity
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public int QuestionTypeId { get; set; }

    [Required(ErrorMessage = "Nội dung câu hỏi không được để trống")]
    public string Content { get; set; } = string.Empty;

    public AssessmentTest Test { get; set; } = null!;
    public QuestionType QuestionType { get; set; } = null!;
    public ICollection<QuestionOption> Options { get; set; } = [];
    public ICollection<TestAnswer> TestAnswers { get; set; } = [];
}

using System.ComponentModel.DataAnnotations;

namespace CareerGuidance.Models;

public class QuestionOption : BaseEntity
{
    public int Id { get; set; }
    public int QuestionId { get; set; }

    [Required(ErrorMessage = "Nội dung đáp án không được để trống")]
    [StringLength(500)]
    public string Content { get; set; } = string.Empty;

    public QuestionTest Question { get; set; } = null!;
    public ICollection<OptionCareerPath> OptionCareerPaths { get; set; } = [];
    public ICollection<TestAnswer> TestAnswers { get; set; } = [];
}

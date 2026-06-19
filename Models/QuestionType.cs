using System.ComponentModel.DataAnnotations;

namespace CareerGuidance.Models;

public class QuestionType : BaseEntity
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên loại câu hỏi không được để trống")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }

    public ICollection<QuestionTest> QuestionTests { get; set; } = [];
}

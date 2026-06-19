using System.ComponentModel.DataAnnotations;

namespace CareerGuidance.Models;

public class Goal : BaseEntity
{
    public int Id { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public int? CareerPathId { get; set; }

    [Required(ErrorMessage = "Tiêu đề mục tiêu không được để trống")]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [Range(0, 100, ErrorMessage = "Tiến độ phải từ 0 đến 100")]
    public int Progress { get; set; }

    public DateTime? TargetDate { get; set; }

    public AppUser Student { get; set; } = null!;
    public CareerPath? CareerPath { get; set; }
}

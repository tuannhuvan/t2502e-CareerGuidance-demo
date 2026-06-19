using System.ComponentModel.DataAnnotations;

namespace CareerGuidance.Models;

public class JobPosting : BaseEntity
{
    public int Id { get; set; }
    public int? CareerPathId { get; set; }

    [Required(ErrorMessage = "Tiêu đề tin tuyển dụng không được để trống")]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [StringLength(255)]
    public string? CompanyName { get; set; }

    public decimal Salary { get; set; }
    public string? Description { get; set; }

    public CareerPath? CareerPath { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace CareerGuidance.Models;

public class CareerPath : BaseEntity
{
    public int Id { get; set; }
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Tiêu đề lộ trình không được để trống")]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    public Category Category { get; set; } = null!;
    public ICollection<OptionCareerPath> OptionCareerPaths { get; set; } = [];
    public ICollection<TestResult> TestResults { get; set; } = [];
    public ICollection<Resource> Resources { get; set; } = [];
    public ICollection<JobPosting> JobPostings { get; set; } = [];
    public ICollection<Goal> Goals { get; set; } = [];
}

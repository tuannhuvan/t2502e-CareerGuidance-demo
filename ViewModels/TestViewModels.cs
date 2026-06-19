using CareerGuidance.Helpers;
using CareerGuidance.Models;

namespace CareerGuidance.ViewModels;

public class TestFilterViewModel
{
    public string? Keyword { get; set; }
    public string? DateRange { get; set; }
    public string? SortOrder { get; set; }
    public int? Status { get; set; }
    public PaginatedList<AssessmentTest> Tests { get; set; } = new([], 0, 1, 10);
}

public class TestFormViewModel
{
    public int Id { get; set; }

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Tiêu đề bài test không được để trống")]
    [System.ComponentModel.DataAnnotations.StringLength(255)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [System.ComponentModel.DataAnnotations.Range(0, 3)]
    public int Status { get; set; } = 1;
}

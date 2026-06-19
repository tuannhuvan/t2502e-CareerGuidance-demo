using CareerGuidance.Helpers;
using CareerGuidance.Models;

namespace CareerGuidance.ViewModels;

public class CategoryFilterViewModel
{
    public string? Keyword { get; set; }
    public string? DateRange { get; set; }
    public string? SortOrder { get; set; }
    public PaginatedList<Category> Categories { get; set; } = new([], 0, 1, 10);
}

public class CategoryFormViewModel
{
    public int Id { get; set; }

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Tên danh mục không được để trống")]
    [System.ComponentModel.DataAnnotations.StringLength(255)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [System.ComponentModel.DataAnnotations.Range(0, 3)]
    public int Status { get; set; } = 1;
}

public class CareerPathFilterViewModel
{
    public string? Keyword { get; set; }
    public int? CategoryId { get; set; }
    public string? SortOrder { get; set; }
    public PaginatedList<CareerPath> CareerPaths { get; set; } = new([], 0, 1, 10);
    public List<Category> CategoryOptions { get; set; } = [];
}

public class CareerPathFormViewModel
{
    public int Id { get; set; }

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng chọn danh mục")]
    public int CategoryId { get; set; }

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Tiêu đề lộ trình không được để trống")]
    [System.ComponentModel.DataAnnotations.StringLength(255)]
    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    [System.ComponentModel.DataAnnotations.Range(0, 3)]
    public int Status { get; set; } = 1;

    public List<Category> CategoryOptions { get; set; } = [];
}

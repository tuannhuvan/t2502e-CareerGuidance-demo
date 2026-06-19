using System.ComponentModel.DataAnnotations;

namespace CareerGuidance.Models;

public class Category : BaseEntity
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên danh mục không được để trống")]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<CareerPath> CareerPaths { get; set; } = [];
}

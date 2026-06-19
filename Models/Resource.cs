using System.ComponentModel.DataAnnotations;

namespace CareerGuidance.Models;

public class Resource : BaseEntity
{
    public int Id { get; set; }
    public int PathId { get; set; }

    [StringLength(50)]
    public string? ResourceType { get; set; }

    [StringLength(255)]
    public string? Title { get; set; }

    [StringLength(500)]
    public string? Url { get; set; }

    public CareerPath CareerPath { get; set; } = null!;
}

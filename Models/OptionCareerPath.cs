namespace CareerGuidance.Models;

public class OptionCareerPath : BaseEntity
{
    public int Id { get; set; }
    public int OptionId { get; set; }
    public int CareerPathId { get; set; }
    public int Weight { get; set; } = 1;

    public QuestionOption Option { get; set; } = null!;
    public CareerPath CareerPath { get; set; } = null!;
}

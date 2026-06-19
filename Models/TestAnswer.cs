namespace CareerGuidance.Models;

public class TestAnswer : BaseEntity
{
    public int Id { get; set; }
    public int ResultId { get; set; }
    public int QuestionId { get; set; }
    public int OptionId { get; set; }

    public TestResult Result { get; set; } = null!;
    public QuestionTest Question { get; set; } = null!;
    public QuestionOption Option { get; set; } = null!;
}

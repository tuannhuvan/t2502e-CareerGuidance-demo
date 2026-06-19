using Microsoft.AspNetCore.Identity;

namespace CareerGuidance.Models;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int Status { get; set; } = 1;

    public ICollection<TestResult> TestResults { get; set; } = [];
    public ICollection<Goal> Goals { get; set; } = [];
}

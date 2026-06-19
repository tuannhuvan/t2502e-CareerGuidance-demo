using CareerGuidance.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CareerGuidance.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<QuestionType> QuestionTypes => Set<QuestionType>();
    public DbSet<AssessmentTest> Tests => Set<AssessmentTest>();
    public DbSet<QuestionTest> QuestionTests => Set<QuestionTest>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CareerPath> CareerPaths => Set<CareerPath>();
    public DbSet<OptionCareerPath> OptionCareerPaths => Set<OptionCareerPath>();
    public DbSet<TestResult> TestResults => Set<TestResult>();
    public DbSet<TestAnswer> TestAnswers => Set<TestAnswer>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<JobPosting> JobPostings => Set<JobPosting>();
    public DbSet<Goal> Goals => Set<Goal>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>(entity =>
        {
            entity.Property(u => u.FullName).HasMaxLength(255);
            entity.Property(u => u.Address).HasMaxLength(500);
        });

        ConfigureAuditEntity<QuestionType>(builder, "question_types");
        ConfigureAuditEntity<AssessmentTest>(builder, "tests");
        ConfigureAuditEntity<QuestionTest>(builder, "question_tests");
        ConfigureAuditEntity<QuestionOption>(builder, "question_options");
        ConfigureAuditEntity<Category>(builder, "categories");
        ConfigureAuditEntity<CareerPath>(builder, "career_paths");
        ConfigureAuditEntity<OptionCareerPath>(builder, "option_career_paths");
        ConfigureAuditEntity<TestResult>(builder, "test_results");
        ConfigureAuditEntity<TestAnswer>(builder, "test_answers");
        ConfigureAuditEntity<Resource>(builder, "resources");
        ConfigureAuditEntity<JobPosting>(builder, "job_postings");
        ConfigureAuditEntity<Goal>(builder, "goals");

        builder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        builder.Entity<QuestionTest>()
            .HasOne(q => q.Test)
            .WithMany(t => t.Questions)
            .HasForeignKey(q => q.TestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<QuestionTest>()
            .HasOne(q => q.QuestionType)
            .WithMany(t => t.QuestionTests)
            .HasForeignKey(q => q.QuestionTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<QuestionOption>()
            .HasOne(o => o.Question)
            .WithMany(q => q.Options)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CareerPath>()
            .HasOne(c => c.Category)
            .WithMany(c => c.CareerPaths)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<OptionCareerPath>()
            .HasOne(o => o.Option)
            .WithMany(o => o.OptionCareerPaths)
            .HasForeignKey(o => o.OptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<OptionCareerPath>()
            .HasOne(o => o.CareerPath)
            .WithMany(c => c.OptionCareerPaths)
            .HasForeignKey(o => o.CareerPathId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<OptionCareerPath>()
            .HasIndex(o => new { o.OptionId, o.CareerPathId })
            .IsUnique();

        builder.Entity<TestResult>()
            .HasOne(r => r.Student)
            .WithMany(u => u.TestResults)
            .HasForeignKey(r => r.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TestResult>()
            .HasOne(r => r.Test)
            .WithMany(t => t.TestResults)
            .HasForeignKey(r => r.TestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TestResult>()
            .HasOne(r => r.RecommendedCareerPath)
            .WithMany(c => c.TestResults)
            .HasForeignKey(r => r.RecommendedCareerPathId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<TestAnswer>()
            .HasOne(a => a.Result)
            .WithMany(r => r.TestAnswers)
            .HasForeignKey(a => a.ResultId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TestAnswer>()
            .HasOne(a => a.Question)
            .WithMany(q => q.TestAnswers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TestAnswer>()
            .HasOne(a => a.Option)
            .WithMany(o => o.TestAnswers)
            .HasForeignKey(a => a.OptionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Resource>()
            .HasOne(r => r.CareerPath)
            .WithMany(c => c.Resources)
            .HasForeignKey(r => r.PathId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<JobPosting>()
            .HasOne(j => j.CareerPath)
            .WithMany(c => c.JobPostings)
            .HasForeignKey(j => j.CareerPathId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Goal>()
            .HasOne(g => g.Student)
            .WithMany(u => u.Goals)
            .HasForeignKey(g => g.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Goal>()
            .HasOne(g => g.CareerPath)
            .WithMany(c => c.Goals)
            .HasForeignKey(g => g.CareerPathId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private static void ConfigureAuditEntity<T>(ModelBuilder builder, string tableName) where T : BaseEntity
    {
        builder.Entity<T>(entity =>
        {
            entity.ToTable(tableName);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
            entity.Property(e => e.Status).HasColumnName("status");
        });
    }
}

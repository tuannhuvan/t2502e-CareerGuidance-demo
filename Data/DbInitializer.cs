using CareerGuidance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CareerGuidance.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        await SeedRolesAndAdminAsync(roleManager, userManager);
        await SeedBusinessDataAsync(context);
    }

    private static async Task SeedRolesAndAdminAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<AppUser> userManager)
    {
        string[] roleNames = ["Admin", "Mentor", "Student"];

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        const string adminEmail = "admin@careerguidance.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        const string studentEmail = "student@careerguidance.com";
        if (await userManager.FindByEmailAsync(studentEmail) == null)
        {
            var student = new AppUser
            {
                UserName = studentEmail,
                Email = studentEmail,
                FullName = "Nguyễn Văn Học Sinh",
                EmailConfirmed = true
            };

            if ((await userManager.CreateAsync(student, "Student@123")).Succeeded)
            {
                await userManager.AddToRoleAsync(student, "Student");
            }
        }
    }

    private static async Task SeedBusinessDataAsync(AppDbContext context)
    {
        if (!await context.QuestionTypes.AnyAsync())
        {
            context.QuestionTypes.AddRange(
                new QuestionType { Name = "SingleChoice", Description = "Một đáp án duy nhất" },
                new QuestionType { Name = "MultiChoice", Description = "Nhiều đáp án" },
                new QuestionType { Name = "YesNo", Description = "Có / Không" });
            await context.SaveChangesAsync();
        }

        if (await context.Categories.AnyAsync())
        {
            return;
        }

        var singleChoice = await context.QuestionTypes.FirstAsync(q => q.Name == "SingleChoice");
        var yesNo = await context.QuestionTypes.FirstAsync(q => q.Name == "YesNo");

        var techCategory = new Category
        {
            Name = "Công nghệ thông tin",
            Description = "Ngành nghề liên quan phần mềm, dữ liệu và hạ tầng số"
        };
        var businessCategory = new Category
        {
            Name = "Kinh doanh & Marketing",
            Description = "Quản trị, bán hàng và truyền thông thương hiệu"
        };
        context.Categories.AddRange(techCategory, businessCategory);
        await context.SaveChangesAsync();

        var softwarePath = new CareerPath
        {
            CategoryId = techCategory.Id,
            Title = "Lập trình viên Fullstack",
            Content = "<p>Phát triển ứng dụng web end-to-end với .NET và JavaScript.</p>"
        };
        var dataPath = new CareerPath
        {
            CategoryId = techCategory.Id,
            Title = "Chuyên viên Phân tích Dữ liệu",
            Content = "<p>Phân tích dữ liệu hỗ trợ ra quyết định kinh doanh.</p>"
        };
        var marketingPath = new CareerPath
        {
            CategoryId = businessCategory.Id,
            Title = "Digital Marketing Specialist",
            Content = "<p>Triển khai chiến dịch quảng cáo số đa kênh.</p>"
        };
        context.CareerPaths.AddRange(softwarePath, dataPath, marketingPath);
        await context.SaveChangesAsync();

        var hollandTest = new AssessmentTest
        {
            Title = "Trắc nghiệm Holland (Mẫu)",
            Description = "Bài đánh giá xu hướng nghề nghiệp RIASEC - dữ liệu mẫu cho team phát triển."
        };
        context.Tests.Add(hollandTest);
        await context.SaveChangesAsync();

        var question1 = new QuestionTest
        {
            TestId = hollandTest.Id,
            QuestionTypeId = singleChoice.Id,
            Content = "Bạn thích làm việc với máy tính và viết mã nguồn?"
        };
        var question2 = new QuestionTest
        {
            TestId = hollandTest.Id,
            QuestionTypeId = yesNo.Id,
            Content = "Bạn có hứng thú với việc phân tích số liệu và báo cáo?"
        };
        context.QuestionTests.AddRange(question1, question2);
        await context.SaveChangesAsync();

        var opt1a = new QuestionOption { QuestionId = question1.Id, Content = "Rất thích - muốn theo đuổi lâu dài" };
        var opt1b = new QuestionOption { QuestionId = question1.Id, Content = "Bình thường" };
        var opt2a = new QuestionOption { QuestionId = question2.Id, Content = "Có" };
        var opt2b = new QuestionOption { QuestionId = question2.Id, Content = "Không" };
        context.QuestionOptions.AddRange(opt1a, opt1b, opt2a, opt2b);
        await context.SaveChangesAsync();

        context.OptionCareerPaths.AddRange(
            new OptionCareerPath { OptionId = opt1a.Id, CareerPathId = softwarePath.Id, Weight = 3 },
            new OptionCareerPath { OptionId = opt1b.Id, CareerPathId = softwarePath.Id, Weight = 1 },
            new OptionCareerPath { OptionId = opt2a.Id, CareerPathId = dataPath.Id, Weight = 3 },
            new OptionCareerPath { OptionId = opt2a.Id, CareerPathId = marketingPath.Id, Weight = 1 },
            new OptionCareerPath { OptionId = opt2b.Id, CareerPathId = dataPath.Id, Weight = 0 });

        context.Resources.Add(new Resource
        {
            PathId = softwarePath.Id,
            ResourceType = "Blog",
            Title = "Lộ trình học Fullstack .NET",
            Url = "https://learn.microsoft.com/aspnet/core"
        });

        context.JobPostings.Add(new JobPosting
        {
            CareerPathId = softwarePath.Id,
            Title = "Junior .NET Developer",
            CompanyName = "FPT Software",
            Salary = 12000000,
            Description = "Tuyển fresher có nền tảng C# và SQL."
        });

        await context.SaveChangesAsync();
    }
}

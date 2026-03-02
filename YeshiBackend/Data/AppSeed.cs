using Microsoft.EntityFrameworkCore;
using YeshiBackend.Models;

namespace YeshiBackend.Data;

public static class AppSeed
{
    public static async Task SeedAsync(AppDbContext dbContext, IConfiguration configuration)
    {
        if (!await dbContext.Categories.AnyAsync())
        {
            dbContext.Categories.AddRange(
                new Category { Name = "Habesha Kemis", Description = "Traditional women wear" },
                new Category { Name = "Men Traditional", Description = "Traditional men wear" }
            );
        }

        var adminEmail = (configuration["AdminSeed:Email"] ?? "admin@yeshi.local").Trim().ToLowerInvariant();
        var adminPassword = configuration["AdminSeed:Password"] ?? "Admin@12345";

        var existingAdmin = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == adminEmail);
        if (existingAdmin is null)
        {
            var adminUser = new User
            {
                FullName = "System Admin",
                Email = adminEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            dbContext.Users.Add(adminUser);
            await dbContext.SaveChangesAsync();

            dbContext.Admins.Add(new Admin
            {
                UserId = adminUser.Id,
                Department = "Operations",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        await dbContext.SaveChangesAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using YeshiBackend.Data;
using YeshiBackend.Dtos;
using YeshiBackend.Models;

namespace YeshiBackend.Services;

public interface IAuthService
{
    Task<User> RegisterCustomerAsync(RegisterRequest request);
    Task<User?> ValidateUserAsync(LoginRequest request);
}

public class AuthService(AppDbContext dbContext) : IAuthService
{
    public async Task<User> RegisterCustomerAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await dbContext.Users.AnyAsync(x => x.Email == email))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            Phone = request.Phone?.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "Customer",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User?> ValidateUserAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        return BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash) ? user : null;
    }
}

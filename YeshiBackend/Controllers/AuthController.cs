using Microsoft.AspNetCore.Mvc;
using YeshiBackend.Dtos;
using YeshiBackend.Services;

namespace YeshiBackend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, IJwtTokenService jwtTokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var user = await authService.RegisterCustomerAsync(request);
            return StatusCode(StatusCodes.Status201Created, new
            {
                message = "Registration successful.",
                user = new { id = user.Id, user.FullName, user.Email, user.Role }
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var user = await authService.ValidateUserAsync(request);
        if (user is null)
        {
            return Unauthorized(new { message = "Invalid credentials." });
        }

        var token = jwtTokenService.CreateToken(user);

        return Ok(new AuthResponse(token, new
        {
            id = user.Id,
            user.FullName,
            user.Email,
            user.Role
        }));
    }
}

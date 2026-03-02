using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YeshiBackend.Data;
using YeshiBackend.Dtos;
using YeshiBackend.Models;

namespace YeshiBackend.Controllers;

[ApiController]
[Route("api/admin/categories")]
[Authorize(Roles = "Admin")]
public class AdminCategoriesController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        var categories = dbContext.Categories.OrderBy(x => x.Name).ToList();
        return Ok(new { categories });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var category = new Category
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();
        return StatusCode(StatusCodes.Status201Created, category);
    }
}

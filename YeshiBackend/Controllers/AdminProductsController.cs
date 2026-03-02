using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YeshiBackend.Dtos;
using YeshiBackend.Services;

namespace YeshiBackend.Controllers;

[ApiController]
[Route("api/admin/products")]
[Authorize(Roles = "Admin")]
public class AdminProductsController(ICatalogService catalogService, IWebHostEnvironment environment) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] int? categoryId)
    {
        var products = await catalogService.GetProductsAsync(categoryId);
        return Ok(new { products });
    }

    [HttpPost]
    [RequestSizeLimit(15_000_000)]
    public async Task<IActionResult> Create([FromForm] ProductRequest request, [FromForm] IFormFile? image)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var imagePath = await SaveImageAsync(image);
        var product = await catalogService.CreateProductAsync(request, imagePath);
        return StatusCode(StatusCodes.Status201Created, product);
    }

    [HttpPut("{id:int}")]
    [RequestSizeLimit(15_000_000)]
    public async Task<IActionResult> Update(int id, [FromForm] ProductRequest request, [FromForm] IFormFile? image)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var imagePath = await SaveImageAsync(image);
        var product = await catalogService.UpdateProductAsync(id, request, imagePath);
        return product is null ? NotFound(new { message = "Product not found." }) : Ok(product);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await catalogService.DeleteProductAsync(id);
        return ok ? NoContent() : NotFound(new { message = "Product not found." });
    }

    private async Task<string?> SaveImageAsync(IFormFile? image)
    {
        if (image is null || image.Length == 0) return null;

        var extension = Path.GetExtension(image.FileName);
        var fileName = $"product-{Guid.NewGuid():N}{extension}";
        var directory = Path.Combine(environment.ContentRootPath, "Uploads", "products");
        Directory.CreateDirectory(directory);

        var path = Path.Combine(directory, fileName);
        await using var stream = new FileStream(path, FileMode.CreateNew);
        await image.CopyToAsync(stream);

        return $"/uploads/products/{fileName}";
    }
}

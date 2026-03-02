using Microsoft.AspNetCore.Mvc;
using YeshiBackend.Services;

namespace YeshiBackend.Controllers;

[ApiController]
[Route("api/catalog")]
public class CatalogController(ICatalogService catalogService) : ControllerBase
{
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await catalogService.GetCategoriesAsync();
        return Ok(new { categories });
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts([FromQuery] int? categoryId)
    {
        var products = await catalogService.GetProductsAsync(categoryId);
        return Ok(new { products });
    }

    [HttpGet("products/{id:int}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await catalogService.GetProductAsync(id);
        return product is null ? NotFound(new { message = "Product not found." }) : Ok(product);
    }
}

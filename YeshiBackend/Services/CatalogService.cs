using Microsoft.EntityFrameworkCore;
using YeshiBackend.Data;
using YeshiBackend.Dtos;
using YeshiBackend.Models;

namespace YeshiBackend.Services;

public interface ICatalogService
{
    Task<List<Category>> GetCategoriesAsync();
    Task<List<Product>> GetProductsAsync(int? categoryId);
    Task<Product?> GetProductAsync(int id);
    Task<Product> CreateProductAsync(ProductRequest request, string? imagePath);
    Task<Product?> UpdateProductAsync(int id, ProductRequest request, string? imagePath);
    Task<bool> DeleteProductAsync(int id);
}

public class CatalogService(AppDbContext dbContext) : ICatalogService
{
    public Task<List<Category>> GetCategoriesAsync()
    {
        return dbContext.Categories.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
    }

    public Task<List<Product>> GetProductsAsync(int? categoryId)
    {
        var query = dbContext.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.CreatedAt)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == categoryId.Value);
        }

        return query.ToListAsync();
    }

    public Task<Product?> GetProductAsync(int id)
    {
        return dbContext.Products.AsNoTracking().Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Product> CreateProductAsync(ProductRequest request, string? imagePath)
    {
        var product = new Product
        {
            CategoryId = request.CategoryId,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price,
            ImagePath = imagePath,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateProductAsync(int id, ProductRequest request, string? imagePath)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (product is null) return null;

        product.CategoryId = request.CategoryId;
        product.Name = request.Name.Trim();
        product.Description = request.Description?.Trim();
        product.Price = request.Price;
        product.IsActive = request.IsActive;
        if (!string.IsNullOrWhiteSpace(imagePath))
        {
            product.ImagePath = imagePath;
        }
        product.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (product is null) return false;
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();
        return true;
    }
}

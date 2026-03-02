using System.ComponentModel.DataAnnotations;

namespace YeshiBackend.Dtos;

public class CategoryRequest
{
    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }
}

public class ProductRequest
{
    [Required]
    public int CategoryId { get; set; }

    [Required, MaxLength(160)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Range(0, 99999999)]
    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;
}

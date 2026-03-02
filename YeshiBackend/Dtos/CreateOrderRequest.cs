using Microsoft.AspNetCore.Mvc;

namespace YeshiBackend.Dtos;

public class CreateOrderRequest
{
    [FromForm(Name = "fullName")]
    public string? FullName { get; set; }

    [FromForm(Name = "phone")]
    public string? Phone { get; set; }

    [FromForm(Name = "region")]
    public string? Region { get; set; }

    [FromForm(Name = "city")]
    public string? City { get; set; }

    [FromForm(Name = "address")]
    public string? Address { get; set; }

    [FromForm(Name = "category")]
    public string? Category { get; set; }

    [FromForm(Name = "color")]
    public string? Color { get; set; }

    [FromForm(Name = "size")]
    public string? Size { get; set; }

    [FromForm(Name = "deliveryMethod")]
    public string? DeliveryMethod { get; set; }

    [FromForm(Name = "paymentScreenshot")]
    public IFormFile? PaymentScreenshot { get; set; }

    [FromForm(Name = "referenceImages")]
    public List<IFormFile>? ReferenceImages { get; set; }
}

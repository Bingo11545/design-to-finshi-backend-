using System.ComponentModel.DataAnnotations;

namespace YeshiBackend.Dtos;

public class AddToCartRequest
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, 1000)]
    public int Quantity { get; set; } = 1;
}

public class UpdateCartItemRequest
{
    [Range(1, 1000)]
    public int Quantity { get; set; }
}

public class CheckoutRequest
{
    [Required, MaxLength(500)]
    public string ShippingAddress { get; set; } = string.Empty;
}

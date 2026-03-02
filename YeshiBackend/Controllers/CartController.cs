using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YeshiBackend.Dtos;
using YeshiBackend.Services;

namespace YeshiBackend.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize(Roles = "Customer")]
public class CartController(ISessionCartService cartService, IOrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var cart = await cartService.GetCartAsync(HttpContext.Session);
        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var cart = await cartService.AddAsync(HttpContext.Session, request);
            return Ok(cart);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("items/{productId:int}")]
    public async Task<IActionResult> UpdateItem(int productId, [FromBody] UpdateCartItemRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var cart = await cartService.UpdateAsync(HttpContext.Session, productId, request.Quantity);
            return Ok(cart);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("items/{productId:int}")]
    public async Task<IActionResult> RemoveItem(int productId)
    {
        var cart = await cartService.RemoveAsync(HttpContext.Session, productId);
        return Ok(cart);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var cart = await cartService.GetCartAsync(HttpContext.Session);
        var userId = GetCurrentUserId();

        try
        {
            var order = await orderService.CheckoutAsync(userId, request, cart);
            await cartService.ClearAsync(HttpContext.Session);
            return StatusCode(StatusCodes.Status201Created, new { order.Id, order.Status, order.TotalAmount });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private int GetCurrentUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(sub, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid token subject.");
        }
        return userId;
    }
}

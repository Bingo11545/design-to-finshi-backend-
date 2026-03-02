using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using YeshiBackend.Data;
using YeshiBackend.Dtos;

namespace YeshiBackend.Services;

public record CartLine(int ProductId, string Name, decimal UnitPrice, int Quantity, decimal LineTotal, string? ImagePath);
public record CartView(IReadOnlyCollection<CartLine> Items, decimal Total);

public interface ISessionCartService
{
    Task<CartView> GetCartAsync(ISession session);
    Task<CartView> AddAsync(ISession session, AddToCartRequest request);
    Task<CartView> UpdateAsync(ISession session, int productId, int quantity);
    Task<CartView> RemoveAsync(ISession session, int productId);
    Task ClearAsync(ISession session);
}

public class SessionCartService(AppDbContext dbContext) : ISessionCartService
{
    private const string CartKey = "YESHI_CART";

    public async Task<CartView> GetCartAsync(ISession session)
    {
        var cart = ReadCart(session);
        return await ToViewAsync(cart);
    }

    public async Task<CartView> AddAsync(ISession session, AddToCartRequest request)
    {
        var product = await dbContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.ProductId && x.IsActive);
        if (product is null)
        {
            throw new InvalidOperationException("Product not found.");
        }

        var cart = ReadCart(session);
        if (cart.TryGetValue(request.ProductId, out var existingQty))
        {
            cart[request.ProductId] = existingQty + request.Quantity;
        }
        else
        {
            cart[request.ProductId] = request.Quantity;
        }

        WriteCart(session, cart);
        return await ToViewAsync(cart);
    }

    public async Task<CartView> UpdateAsync(ISession session, int productId, int quantity)
    {
        var cart = ReadCart(session);
        if (!cart.ContainsKey(productId))
        {
            throw new InvalidOperationException("Cart item not found.");
        }

        cart[productId] = quantity;
        WriteCart(session, cart);
        return await ToViewAsync(cart);
    }

    public async Task<CartView> RemoveAsync(ISession session, int productId)
    {
        var cart = ReadCart(session);
        cart.Remove(productId);
        WriteCart(session, cart);
        return await ToViewAsync(cart);
    }

    public Task ClearAsync(ISession session)
    {
        session.Remove(CartKey);
        return Task.CompletedTask;
    }

    private static Dictionary<int, int> ReadCart(ISession session)
    {
        var json = session.GetString(CartKey);
        if (string.IsNullOrWhiteSpace(json)) return new Dictionary<int, int>();
        return JsonSerializer.Deserialize<Dictionary<int, int>>(json) ?? new Dictionary<int, int>();
    }

    private static void WriteCart(ISession session, Dictionary<int, int> cart)
    {
        session.SetString(CartKey, JsonSerializer.Serialize(cart));
    }

    private async Task<CartView> ToViewAsync(Dictionary<int, int> cart)
    {
        if (cart.Count == 0)
        {
            return new CartView(Array.Empty<CartLine>(), 0);
        }

        var ids = cart.Keys.ToList();
        var products = await dbContext.Products.AsNoTracking().Where(x => ids.Contains(x.Id)).ToListAsync();

        var lines = products.Select(p =>
        {
            var qty = cart.TryGetValue(p.Id, out var count) ? count : 0;
            var lineTotal = p.Price * qty;
            return new CartLine(p.Id, p.Name, p.Price, qty, lineTotal, p.ImagePath);
        }).Where(x => x.Quantity > 0).ToList();

        var total = lines.Sum(x => x.LineTotal);
        return new CartView(lines, total);
    }
}

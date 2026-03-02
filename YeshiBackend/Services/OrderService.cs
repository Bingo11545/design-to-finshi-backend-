using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using YeshiBackend.Data;
using YeshiBackend.Dtos;
using YeshiBackend.Models;

namespace YeshiBackend.Services;

public interface IOrderService
{
    Task<Order> CheckoutAsync(int userId, CheckoutRequest request, CartView cart);
    Task<List<Order>> GetUserOrdersAsync(int userId);
    Task<List<Order>> GetAllOrdersAsync();
    Task<Order?> UpdateStatusAsync(int orderId, string status);
}

public class OrderService(AppDbContext dbContext) : IOrderService
{
    private static readonly HashSet<string> AllowedStatuses =
    [
        "Pending",
        "Processing",
        "Shipped",
        "Completed"
    ];

    public async Task<Order> CheckoutAsync(int userId, CheckoutRequest request, CartView cart)
    {
        if (cart.Items.Count == 0)
        {
            throw new InvalidOperationException("Cart is empty.");
        }

        var order = new Order
        {
            UserId = userId,
            ShippingAddress = request.ShippingAddress.Trim(),
            Status = "Pending",
            TotalAmount = cart.Total,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Items = cart.Items.Select(x => new OrderItem
            {
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                LineTotal = x.LineTotal,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList()
        };

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();
        return order;
    }

    public Task<List<Order>> GetUserOrdersAsync(int userId)
    {
        return dbContext.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public Task<List<Order>> GetAllOrdersAsync()
    {
        return dbContext.Orders
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> UpdateStatusAsync(int orderId, string status)
    {
        var normalized = status.Trim();
        if (!AllowedStatuses.Contains(normalized))
        {
            throw new InvalidOperationException("Invalid order status.");
        }

        var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if (order is null) return null;

        order.Status = normalized;
        order.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        return order;
    }
}

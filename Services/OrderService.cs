using App.Models;
using Microsoft.EntityFrameworkCore;
using XEDAPVIP.Models;

public class OrderService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly HttpContext _httpContext;

    public OrderService(AppDbContext context, IHttpContextAccessor contextAccessor)
    {
        _context = context;
        _contextAccessor = contextAccessor;
        _httpContext = contextAccessor.HttpContext;
    }

    // Create a new order
    public async Task<Order> CreateOrderAsync(string? userId, string? userName, string? userEmail, List<CartItem> cartItems, string? shippingAddress, string shippingMethod, string paymentMethod)
    {
        var order = new Order
        {
            UserId = userId,
            UserName = userName,
            UserEmail = userEmail,
            OrderDetails = cartItems.Select(ci => new OrderDetail
            {
                VariantId = ci.VariantId,
                ProductName = ci.Variant.Product.Name,
                ProductDescription = $"Màu sắc: {ci.Variant.Color}, Kích thước: {ci.Variant.Size}",
                ProductImage = ci.Variant.Product.MainImage, // Assuming Product entity has a MainImage property
                Quantity = ci.Quantity,
                UnitPrice = ci.Variant.Product.DiscountPrice ?? ci.Variant.Product.Price
            }).ToList(),
            OrderDate = DateTime.Now,
            TotalAmount = cartItems.Sum(ci => ci.Quantity * (ci.Variant.Product.DiscountPrice ?? ci.Variant.Product.Price)),
            Status = "Pending",
            ShippingAddress = shippingAddress,
            ShippingMethod = shippingMethod,
            PaymentMethod = paymentMethod
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order;
    }

    // Get orders for a user
    public async Task<List<Order>> GetOrdersAsync(string? userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Variant)
            .ThenInclude(v => v.Product)
            .ToListAsync();
    }

    // Get a specific order by ID
    public async Task<Order> GetOrderByIdAsync(int orderId)
    {
        return await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Variant)
            .ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    // Delete an order
    public async Task<bool> DeleteOrderAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
        {
            return false;
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return true;
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using App.Models;
using Microsoft.EntityFrameworkCore;
using XEDAPVIP.Models;

public class CartService
{
    private readonly AppDbContext _context;

    public CartService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cart> GetOrCreateCartAsync(string userId, string sessionId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId || c.SessionId == sessionId);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                SessionId = sessionId,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        return cart;
    }

    public async Task AddToCartAsync(string userId, string sessionId, int productId, string productCode, int quantity, double price)
    {
        var cart = await GetOrCreateCartAsync(userId, sessionId);

        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId && ci.productCode == productCode);
        if (cartItem != null)
        {
            cartItem.Quantity += quantity;
        }
        else
        {
            cartItem = new CartItem
            {
                ProductId = productId,
                productCode = productCode,
                Quantity = quantity,
                Price = price,
                CartId = cart.Id
            };
            cart.CartItems.Add(cartItem);
        }

        cart.DateUpdated = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}

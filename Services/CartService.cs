using Newtonsoft.Json;
using XEDAPVIP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using App.Models;

public class CartService
{
    public const string CARTKEY = "CART";
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly HttpContext _httpContext;

    public CartService(AppDbContext context, IHttpContextAccessor contextAccessor)
    {
        _context = context;
        _contextAccessor = contextAccessor;
        _httpContext = contextAccessor.HttpContext;

    }

    public List<CartItem> GetCartItems(string userId = null)
    {
        if (!string.IsNullOrEmpty(userId))
        {

            return _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Variant)
                .ToList();
        }
        else
        {
            var session = _httpContext.Session;
            string jsonCart = session.GetString(CARTKEY);
            if (jsonCart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsonCart);
            }
            return new List<CartItem>();
        }
    }

    public void ClearCart(string userId = null)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            var cartItems = _context.CartItems.Where(ci => ci.UserId == userId).ToList();
            _context.CartItems.RemoveRange(cartItems);
            _context.SaveChanges();
        }
        else
        {
            var session = _httpContext.Session;
            session.Remove(CARTKEY);
        }
    }

    public void SaveCartItems(string userId, List<CartItem> cartItems)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            var existingCartItems = _context.CartItems.Where(ci => ci.UserId == userId).ToList();
            _context.CartItems.RemoveRange(existingCartItems);
            foreach (var item in cartItems)
            {
                item.UserId = userId;
            }
            _context.CartItems.AddRange(cartItems);
            _context.SaveChanges();
        }
        else
        {
            var session = _httpContext.Session;
            string jsonCart = JsonConvert.SerializeObject(cartItems);
            session.SetString(CARTKEY, jsonCart);
        }
    }
}

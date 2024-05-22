using System.Linq;
using System.Threading.Tasks;
using App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using XEDAPVIP.Models;

namespace App.Areas.Home.Controllers
{
    [Area("Home")]
    public class CartViewController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CartViewController> _logger;
        private readonly CacheService _cacheService;
        private readonly CartService _cartService;

        public CartViewController(AppDbContext context, ILogger<CartViewController> logger, IMemoryCache cache, CacheService cacheService, CartService cartService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
            _cartService = cartService;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(string productCode, int quantity, int productId, double price)
        {
            // Assuming we are using session ID for guest users
            var sessionId = HttpContext.Session.Id;
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : null;

            await _cartService.AddToCartAsync(userId, sessionId, productId, productCode, quantity, price);

            return Json(new { success = true });
        }

        public async Task<IActionResult> Cart()
        {
            var sessionId = HttpContext.Session.Id;
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : null;

            var cart = await _cartService.GetOrCreateCartAsync(userId, sessionId);

            var categories = await _cacheService.GetCategoriesAsync();
            var brands = await _cacheService.GetBrandsAsync();

            ViewBag.categories = categories;
            ViewBag.brands = brands;
            ViewBag.TotalAmount = cart.CartItems.Sum(ci => ci.Price * ci.Quantity).ToString("N0");

            return View(cart);
        }
    }
}

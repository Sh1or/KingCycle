using App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Home.Controllers
{
    [Area("Home")]
    public class ProductViewController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductViewController> _logger;
        private readonly CacheService _cacheService;



        public ProductViewController(AppDbContext context, ILogger<ProductViewController> logger, IMemoryCache cache, CacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;

        }



        [Route("/product/{categoryslug?}")]
        public async Task<IActionResult> Product(string categoryslug, string brandslug, [FromQuery(Name = "p")] int currentPage, int pagesize)
        {
            var categories = await _cacheService.GetCategoriesAsync();
            var brands = await _cacheService.GetBrandsAsync();

            ViewBag.categories = categories;
            ViewBag.categoryslug = categoryslug;
            ViewBag.brands = brands;
            ViewBag.brandslug = brandslug;

            Category category = null;

            if (!string.IsNullOrEmpty(categoryslug))
            {
                category = await _context.Categories
                    .Where(c => c.Slug == categoryslug)
                    .Include(c => c.CategoryChildren)
                    .FirstOrDefaultAsync();

                if (category == null)
                {
                    return NotFound("Không tìm thấy");
                }
            }

            var products = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Variants)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(brandslug))
            {
                products = products.Where(p => p.Brand.Slug == brandslug);
            }

            if (category != null)
            {
                products = products.Where(p => p.ProductCategories.Any(pc => pc.CategoryId == category.Id));
            }

            products = products.OrderByDescending(p => p.DateCreated);

            int totalProducts = await products.CountAsync();
            if (pagesize <= 0) pagesize = 9;
            int countPages = (int)Math.Ceiling((double)totalProducts / pagesize);
            if (currentPage > countPages)
                currentPage = countPages;
            if (currentPage < 1)
                currentPage = 1;

            var pagingmodel = new PagingModel()
            {
                countpages = countPages,
                currentpage = currentPage,
                generateUrl = (pageNumber) => Url.Action("Product", new
                {
                    categoryslug,
                    brandslug,
                    p = pageNumber,
                    pagesize
                })
            };

            var productinPage = await products.Skip((currentPage - 1) * pagesize)
                                        .Take(pagesize)
                                        .ToListAsync();

            ViewBag.pagingmodel = pagingmodel;
            ViewBag.totalProduc = totalProducts;
            ViewBag.category = category;
            return View(productinPage);
        }

        [Route("/product/{categoryslug}/{productslug}.cshtml")]
        public async Task<IActionResult> DetailProduct(string categoryslug, string brandslug, string productslug)
        {
            var categories = await _cacheService.GetCategoriesAsync();
            var brands = await _cacheService.GetBrandsAsync();

            ViewBag.categories = categories;
            ViewBag.categoryslug = categoryslug;
            ViewBag.brands = brands;
            ViewBag.brandslug = brandslug;

            var product = await _context.Products
                .Where(p => p.Slug == productslug)
                .Include(p => p.Brand)
                .Include(p => p.Variants)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm");
            }

            ViewBag.product = product;
            return View(product);
        }















    }
}

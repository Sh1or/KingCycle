using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;
using App.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using App.Utilities;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using XEDAPVIP.Areas.Admin.ModelsProduct;
using System.Diagnostics;
using XEDAPVIP.Models;
using App.Components;
using Microsoft.Extensions.Caching.Memory;

namespace App.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<HomeController> _logger;
    private IMemoryCache _cache;


    public HomeController(ILogger<HomeController> logger,
            AppDbContext context,
            IMemoryCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }


    [NonAction]
    List<Category> GetCategories()
    {

        List<Category> categories;

        string keycacheCategories = "_listallcategories";

        // Phục hồi categories từ Memory cache, không có thì truy vấn Db
        if (!_cache.TryGetValue(keycacheCategories, out categories))
        {

            categories = _context.Categories
                .Include(c => c.CategoryChildren)
                .AsEnumerable()
                .Where(c => c.ParentCategory == null)
                .ToList();

            // Thiết lập cache - lưu vào cache
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(300));
            _cache.Set("_GetCategories", categories, cacheEntryOptions);
        }

        return categories;
    }


    // Tìm (đệ quy) trong cây, một Category theo Slug
    [NonAction]
    Category FindCategoryBySlug(List<Category> categories, string Slug)
    {

        foreach (var c in categories)
        {
            if (c.Slug == Slug) return c;
            var c1 = FindCategoryBySlug(c.CategoryChildren.ToList(), Slug);
            if (c1 != null)
                return c1;
        }

        return null;
    }

    [NonAction]
    List<Brand> GetBrands()
    {
        List<Brand> brands;
        string keyCacheBrands = "_listAllBrands";

        if (!_cache.TryGetValue(keyCacheBrands, out brands))
        {
            brands = _context.Brands.ToList();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(300));
            _cache.Set(keyCacheBrands, brands, cacheEntryOptions);
        }

        return brands;
    }


    public IActionResult Index(string categoryslug, string brandslug)
    {
        var categories = GetCategories();
        var brands = GetBrands();

        ViewBag.categories = categories;
        ViewBag.categoryslug = categoryslug;
        ViewBag.brands = brands;
        ViewBag.brandslug = brandslug;

        return View();
    }

    [Route("/product/{slug?}")]
    public async Task<IActionResult> Product(string categoryslug, string brandslug)
    {
        var categories = GetCategories();
        var brands = GetBrands();

        ViewBag.categories = categories;
        ViewBag.categoryslug = categoryslug;
        ViewBag.brands = brands;
        ViewBag.brandslug = brandslug;

        Category category = null;

        if (!string.IsNullOrEmpty(categoryslug))
        {
            category = _context.Categories.Where(c => c.Slug == categoryslug)
                                          .Include(c => c.CategoryChildren)
                                          .FirstOrDefault();

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

        ViewBag.category = category;
        return View(await products.ToListAsync());
    }


    public IActionResult Privacy(string categoryslug, string brandslug)
    {
        var categories = GetCategories();
        var brands = GetBrands();

        ViewBag.categories = categories;
        ViewBag.categoryslug = categoryslug;
        ViewBag.brands = brands;
        ViewBag.brandslug = brandslug;
        return View();
    }
    public IActionResult Sale(string categoryslug, string brandslug)
    {
        var categories = GetCategories();
        var brands = GetBrands();

        ViewBag.categories = categories;
        ViewBag.categoryslug = categoryslug;
        ViewBag.brands = brands;
        ViewBag.brandslug = brandslug;
        return View();
    }

    public IActionResult Service(string categoryslug, string brandslug)
    {
        var categories = GetCategories();
        var brands = GetBrands();

        ViewBag.categories = categories;
        ViewBag.categoryslug = categoryslug;
        ViewBag.brands = brands;
        ViewBag.brandslug = brandslug;
        return View();
    }
    public IActionResult Cart(string categoryslug, string brandslug)
    {
        var categories = GetCategories();
        var brands = GetBrands();

        ViewBag.categories = categories;
        ViewBag.categoryslug = categoryslug;
        ViewBag.brands = brands;
        ViewBag.brandslug = brandslug;
        return View();
    }

    public IActionResult Check_out(string categoryslug, string brandslug)
    {
        var categories = GetCategories();
        var brands = GetBrands();

        ViewBag.categories = categories;
        ViewBag.categoryslug = categoryslug;
        ViewBag.brands = brands;
        ViewBag.brandslug = brandslug;
        return View();
    }
    [Route("/product/{categoryslug?}/{brandslug?}/{productslug?}")]
    public IActionResult Product_information(string categoryslug, string brandslug, string productslug)
    {
        var categories = GetCategories();
        var brands = GetBrands();

        ViewBag.categories = categories;
        ViewBag.categoryslug = categoryslug;
        ViewBag.brands = brands;
        ViewBag.brandslug = brandslug;

        if (string.IsNullOrEmpty(productslug))
        {
            return NotFound("Product not found");
        }

        var product = _context.Products
                              .Include(p => p.Brand)
                              .Include(p => p.ProductCategories)
                              .Include(p => p.Variants) // Include the variants
                              .FirstOrDefault(p => p.Slug == productslug);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        return View(product);
    }

    public IActionResult Product_select(string categoryslug, string brandslug)
    {
        var categories = GetCategories();
        var brands = GetBrands();

        ViewBag.categories = categories;
        ViewBag.categoryslug = categoryslug;
        ViewBag.brands = brands;
        ViewBag.brandslug = brandslug;
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

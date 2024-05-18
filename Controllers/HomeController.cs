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


    public IActionResult Index(string categoryslug)
    {
        var categories = GetCategories();
        ViewBag.categories = categories;
        ViewBag.categoryslug = categoryslug;
        return View();
    }
    [Route("/product/{categoryslug?}")]
    public async Task<IActionResult> Product(string categoryslug)
    {
        var categories = GetCategories();
        ViewBag.categories = categories;
        ViewBag.categoryslug = categoryslug;

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
        ViewBag.category = category;
        return View();
    }

    public IActionResult Privacy(string categoryslug)
    {
        var categories = GetCategories();
        ViewBag.categories = categories;
        ViewBag.categoryslug = categoryslug;
        return View();
    }
    public IActionResult Sale(string categoryslug)
    {
        var categories = GetCategories();
        ViewBag.categories = categories;
        ViewBag.categoryslug = categoryslug;
        return View();
    }

    public IActionResult Service(string categoryslug)
    {
        var categories = GetCategories();
        ViewBag.categories = categories;
        ViewBag.categoryslug = categoryslug;
        return View();
    }
    public IActionResult Cart(string categoryslug)
    {
        var categories = GetCategories();
        ViewBag.categories = categories;
        ViewBag.categoryslug = categoryslug;
        return View();
    }

    public IActionResult Check_out(string categoryslug)
    {
        var categories = GetCategories();
        ViewBag.categories = categories;
        ViewBag.categoryslug = categoryslug;
        return View();
    }

    public IActionResult Product_information(string categoryslug)
    {
        var categories = GetCategories();
        ViewBag.categories = categories;
        ViewBag.categoryslug = categoryslug;
        return View();
    }

    public IActionResult Product_select(string categoryslug)
    {
        var categories = GetCategories();
        ViewBag.categories = categories;
        ViewBag.categoryslug = categoryslug;
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

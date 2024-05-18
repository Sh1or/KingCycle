using System.Data.Entity;
using System.Diagnostics;
using App.Models;
using Microsoft.AspNetCore.Mvc;
using XEDAPVIP.Models;

namespace App.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<HomeController> _logger;


    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;

    }
    private List<Category> GetCategories()
    {
        var categories = _context.Categories
                        .Include(c => c.CategoryChildren)
                        .AsEnumerable()
                        .Where(c => c.ParentCategory == null)
                        .ToList();
        return categories;
    }

    public IActionResult Index(string categoryslug)
    {
        var category = GetCategories();
        ViewBag.category = category;
        ViewBag.categorySlug = categoryslug;
        return View();
    }

    public IActionResult Product(string categoryslug)
    {
        var category = GetCategories();
        ViewBag.category = category;
        ViewBag.categorySlug = categoryslug;
        return View();
    }
    public IActionResult Privacy(string categoryslug)
    {
        var category = GetCategories();
        ViewBag.category = category;
        ViewBag.categorySlug = categoryslug;
        return View();
    }
    public IActionResult Sale(string categoryslug)
    {
        var category = GetCategories();
        ViewBag.category = category;
        ViewBag.categorySlug = categoryslug;
        return View();
    }

    public IActionResult Service(string categoryslug)
    {
        var category = GetCategories();
        ViewBag.category = category;
        ViewBag.categorySlug = categoryslug;
        return View();
    }
    public IActionResult Cart(string categoryslug)
    {
        var category = GetCategories();
        ViewBag.category = category;
        ViewBag.categorySlug = categoryslug;
        return View();
    }

    public IActionResult Check_out(string categoryslug)
    {
        var category = GetCategories();
        ViewBag.category = category;
        ViewBag.categorySlug = categoryslug;
        return View();
    }

    public IActionResult Product_information(string categoryslug)
    {
        var category = GetCategories();
        ViewBag.category = category;
        ViewBag.categorySlug = categoryslug;
        return View();
    }

    public IActionResult Product_select(string categoryslug)
    {
        var category = GetCategories();
        ViewBag.category = category;
        ViewBag.categorySlug = categoryslug;
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

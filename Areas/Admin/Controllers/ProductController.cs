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
using XEDAPVIP.Areas.Admin.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace XEDAPVIP.Areas.Admin.Controllers
{
    [Authorize(Roles = RoleName.Administrator)]
    [Area("Admin")]
    [Route("Admin/Product/[action]")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ProductController(AppDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Product
        public async Task<IActionResult> Index([FromQuery(Name = "p")] int currentPage, int pagesize)
        {
            var products = _context.Products.OrderByDescending(p => p.DateCreated);
            int totalProduc = await products.CountAsync();
            if (pagesize <= 0) pagesize = 10;
            int countPages = (int)Math.Ceiling((double)totalProduc / pagesize);
            if (currentPage > countPages)
                currentPage = countPages;
            if (currentPage < 1)
                currentPage = 1;
            var pagingmodel = new PagingModel()
            {
                countpages = countPages,
                currentpage = currentPage,
                generateUrl = (pageNumber) => Url.Action("Index", new
                {
                    p = pageNumber,
                    pagesize = pagesize
                })
            };
            ViewBag.pagingmodel = pagingmodel;
            ViewBag.totalProduc = totalProduc;

            var productinPage = await products.Skip((currentPage - 1) * pagesize)
                                              .Take(pagesize)
                                              .Include(p => p.ProductCategories)
                                              .ThenInclude(pc => pc.Category)
                                              .ToListAsync();

            return View(productinPage);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        [BindProperty]
        public int[] selectedCategories { set; get; }
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");

            var brands = await _context.Brands.ToListAsync();
            ViewBag.brands = new SelectList(brands, "Id", "Name");

            var productDetails = new List<ProductDetailEntry>();
            productDetails.Add(new ProductDetailEntry());

            var model = new CreateProductModel
            {
                ProductDetails = productDetails
            };

            return View(model);
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductModel product, IFormFile mainImage, List<IFormFile> subImages)
        {
            if (product.CategoryId == null || product.CategoryId.Length == 0)
            {
                TempData["StatusMessage"] = "Phải chọn ít nhất một danh mục";
                return await ReinitializeCreateView(product);
            }

            if (product.Variants == null || !product.Variants.Any())
            {
                TempData["StatusMessage"] = "Phải nhập ít nhất một biến thể";
                return await ReinitializeCreateView(product);
            }

            if (mainImage == null || mainImage.Length == 0)
            {
                TempData["StatusMessage"] = "Phải tải lên ảnh chính cho sản phẩm";
                return await ReinitializeCreateView(product);
            }

            product.Slug = Utils.GenerateSlug(product.Name);
            ModelState.SetModelValue("Slug", new ValueProviderResult(product.Slug));

            ModelState.Clear();
            TryValidateModel(product);

            if (!ModelState.IsValid)
            {
                return await ReinitializeCreateView(product);
            }

            bool SlugExisted = await _context.Products.AnyAsync(p => p.Slug == product.Slug);
            if (SlugExisted)
            {
                TempData["StatusMessage"] = "Slug đã tồn tại trong cơ sở dữ liệu";
                return await ReinitializeCreateView(product);
            }

            var productDetails = new Dictionary<string, string>();
            foreach (var detail in product.ProductDetails)
            {
                if (string.IsNullOrEmpty(detail.DetailsName) || string.IsNullOrEmpty(detail.DetailsValue))
                {
                    TempData["StatusMessage"] = "Chi tiết sản phẩm không được để trống.";
                    return await ReinitializeCreateView(product);
                }
                productDetails.Add(detail.DetailsName, detail.DetailsValue);
            }
            var detailsJson = JsonConvert.SerializeObject(productDetails);

            try
            {
                var newProduct = new Product
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    DiscountPrice = product.DiscountPrice,
                    Slug = product.Slug,
                    DetailsJson = detailsJson,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    ProductCategories = product.CategoryId.Select(catId => new ProductCategory { CategoryId = catId }).ToList(),
                    Variants = product.Variants.Select(v => new ProductVariant { Color = v.Color, Size = v.Size, Quantity = v.Quantity }).ToList(),
                    BrandId = product.BrandId
                };

                if (mainImage != null && mainImage.Length > 0)
                {
                    var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/images/products/{newProduct.Slug}");
                    Directory.CreateDirectory(directoryPath);

                    var path = Path.Combine(directoryPath, mainImage.FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await mainImage.CopyToAsync(stream);
                    }
                    newProduct.MainImage = mainImage.FileName;
                }

                if (subImages != null && subImages.Count > 0)
                {
                    newProduct.SubImages = new List<string>();
                    foreach (var image in subImages)
                    {
                        if (image.Length > 0)
                        {
                            var fileDirectoryName = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/images/products/{newProduct.Slug}/subImg");
                            Directory.CreateDirectory(fileDirectoryName);

                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                            var filePath = Path.Combine(fileDirectoryName, fileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }
                            newProduct.SubImages.Add(fileName);
                        }
                    }
                }

                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Sản phẩm đã được tạo thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi tạo sản phẩm. Vui lòng thử lại.");
            }

            return await ReinitializeCreateView(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }


            var brands = await _context.Brands.ToListAsync();
            ViewBag.BrandList = new SelectList(brands, "Id", "Name", product.BrandId);

            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");

            return View(product);
        }



        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {


            return View(product);
        }





        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }


        private async Task<IActionResult> ReinitializeCreateView(CreateProductModel product)
        {
            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");

            var brands = await _context.Brands.ToListAsync();
            ViewBag.brands = new SelectList(brands, "Id", "Name");

            return View("Create", product);
        }
    }
}

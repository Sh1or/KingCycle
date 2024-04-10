using System.Net;
using App.ExtendMethods;
using App.Models;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ vào container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    // View\Controller\Action.cshtml
    // MyView\controller\Action.cshtml
    //{0} -> tên action
    //{1} -> tên controller
    //{2} -> tên Area
    options.ViewLocationFormats.Add("/MyView/{1}/{0}" + RazorViewEngine.ViewExtension);
});

// Thêm cấu hình
var appConfiguration = builder.Configuration;

// Thêm DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    string? connectString = appConfiguration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectString);
});

var app = builder.Build();

// Cấu hình pipeline yêu cầu HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Giá trị mặc định HSTS là 30 ngày. Bạn có thể muốn thay đổi điều này cho các kịch bản sản xuất, xem https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.AddStatucCodePage();//tuy bien response khi loi tu 400 -> 599

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>{
    endpoints.MapGet("/sayhi",async (context) => {
        await context.Response.WriteAsync($"Hello ASP.NET MVC {DateTime.Now}");
    });
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();

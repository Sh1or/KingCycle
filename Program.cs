using App.ExtendMethods;
using App.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<RouteOptions>(options =>
{
    options.AppendTrailingSlash = false;
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = false;
});
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add configuration.
var appConfiguration = builder.Configuration;

// Add DbContext.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    string? connectString = appConfiguration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectString);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.AddStatucCodePage();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name : "areas",
    pattern : "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();

app.Run();

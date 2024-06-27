using EcommerceMVC.Data;
using Microsoft.EntityFrameworkCore;
using EcommerceMVC.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Hshop2023Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("HShop"));
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options=>
{
    options.IdleTimeout=TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly=true;
    options.Cookie.IsEssential=true;
});
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-8.0
//Phương thức AddAuthentication được sử dụng để đăng ký middleware xác thực trong đường dẫn ASP.NET Core.
builder.Services.AddAuthentication
    (CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options
    =>
    {
        options.LoginPath = "/KhachHang/DangNhap";//chưa đăng nhập thì chuyển đến trang Đăng nhập
        options.AccessDeniedPath = "/AccessDenied";//Đã đăng nhập rồi thì kiểm tra quyền
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();//xác thực người dùng
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=HomeAdmin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

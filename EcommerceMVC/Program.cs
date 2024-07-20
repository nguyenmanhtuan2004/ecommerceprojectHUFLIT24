using EcommerceMVC.Data;
using Microsoft.EntityFrameworkCore;
using EcommerceMVC.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using EcommerceMVC.Models;
using ECommerceMVC.Helpers;
using EcommerceMVC.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Hshop2023Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("HShop"));
});
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
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
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
options.UseSqlServer(
builder.Configuration["ConnectionStrings:IdentityConnection"]));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
.AddEntityFrameworkStores<AppIdentityDbContext>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
    options.User.RequireUniqueEmail = true;
});

//đăng ký PaypalClient dạng Singleton() - chỉ có 1 instance duy nhất trong toàn ứng dụng
builder.Services.AddSingleton(x => new PaypalClient(
    builder.Configuration["PaypalOptions:AppId"],
    builder.Configuration["PaypalOptions:ClientSecret"],
    builder.Configuration["PaypalOptions:Mode"]
    ));
builder.Services.AddSingleton<IVnPayService, VnPayService>();
var app = builder.Build();

//seed roles
/*using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Customer" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));

        }
    }
}*/

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.MapRazorPages();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();//xác thực người dùng
app.UseAuthorization();
app.MapBlazorHub();
app.MapFallbackToPage("/admin/{*catchall}", "/Admin/Index");
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=HomeAdmin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=HangHoa}/{action=Index}/{id?}");

IdentitySeedData.EnsurePopulated(app);
app.Run();

using Microsoft.EntityFrameworkCore;
using MiniERP.Data;
using MiniERP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using MiniERP.Services;


var builder = WebApplication.CreateBuilder(args);

// Đăng ký dịch vụ MVC
builder.Services.AddControllersWithViews();

// Kết nối database
builder.Services.AddDbContext<MiniERPContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(9, 4)) 
    ));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<MiniERPContext>()
    .AddDefaultTokenProviders();

// Cấu hình cookie cho Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.LoginPath = "/Account/Login";
});

// Cấu hình Kestrel để nghe đúng cổng
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000"; 
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

// Đăng ký HttpContextAccessor để Service có thể đọc được Cookie
builder.Services.AddHttpContextAccessor();

// Đăng ký Service của bạn (dùng AddScoped để tạo mới theo mỗi yêu cầu HTTP)
builder.Services.AddScoped<IJsonLocalizationService, JsonLocalizationService>();

var app = builder.Build();

// Xử lý lỗi
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Phục vụ file tĩnh từ wwwroot
app.UseStaticFiles();

// Phục vụ file tĩnh từ thư mục Images
var imagesPath = Path.Combine(builder.Environment.ContentRootPath, "Images");
if (!Directory.Exists(imagesPath))
{
    Directory.CreateDirectory(imagesPath);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesPath),
    RequestPath = "/Images"
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Middleware kiểm tra login
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();

    if (string.IsNullOrEmpty(path) || path == "/")
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            context.Response.Redirect("/Home/Index");
            return;
        }
        else
        {
            context.Response.Redirect("/Account/Login");
            return;
        }
    }

    await next();
});

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    var adminUser = await userManager.FindByNameAsync("admin");
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = "admin",
            Email = "admin@example.com",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}


// Routing mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

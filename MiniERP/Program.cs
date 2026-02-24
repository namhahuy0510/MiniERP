using Microsoft.EntityFrameworkCore;
using MiniERP.Data;
using MiniERP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MiniERPContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<MiniERPContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Phục vụ file tĩnh từ wwwroot
app.UseStaticFiles();

// Nếu có thư mục riêng (ví dụ "Images"), thêm như sau:
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Images")),
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
            context.Response.Redirect("/dang-nhap");
            return;
        }
    }

    await next();
});

// Routing mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

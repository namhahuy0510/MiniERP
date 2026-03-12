using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using MiniERP.Models;
using MiniERP.Data;
using System;

namespace MyApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MiniERPContext _context;

        public AccountController(SignInManager<ApplicationUser> signInManager,
                                 UserManager<ApplicationUser> userManager,
                                 MiniERPContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Đăng nhập thất bại");
            return View();
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string username, string password, string email, string fullName)
        {
            var user = new ApplicationUser { UserName = username, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Tạo mới Employee gắn với UserId
                var employee = new Employee
                {
                    FullName = fullName,
                    Email = email,
                    Department = "Default",
                    Position = "Staff",
                    DateJoined = DateTime.Now,
                    UserId = user.Id
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }

        // GET: /Account/Logout
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View("~/Views/Shared/AccessDenied.cshtml");
        }

        // POST: /Account/SeedAdmin
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SeedAdmin(string userName, string password)
        {
            var roleManager = HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var adminUser = await userManager.FindByNameAsync(userName);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = userName,
                    Email = $"{userName}@example.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");

                    // Tạo Employee cho Admin
                    var employee = new Employee
                    {
                        FullName = userName,
                        Email = $"{userName}@example.com",
                        Department = "IT",
                        Position = "Administrator",
                        DateJoined = DateTime.Now,
                        UserId = adminUser.Id
                    };
                    _context.Employees.Add(employee);
                    await _context.SaveChangesAsync();

                    return Ok($"Admin user {userName} created successfully.");
                }

                return BadRequest(result.Errors);
            }

            return Ok($"Admin user {userName} already exists.");
        }
    }
}

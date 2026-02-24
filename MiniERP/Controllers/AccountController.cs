using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using MiniERP.Models;

namespace MyApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager,
                                 UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
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
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new ApplicationUser { UserName = username };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
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
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SeedAdmin(string userName, string password)
        {
            var roleManager = HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

            // Tạo role Admin nếu chưa có
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Tạo user Admin nếu chưa có
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
                    return Ok($"Admin user {userName} created successfully.");
                }

                return BadRequest(result.Errors);
            }

            return Ok($"Admin user {userName} already exists.");
        }


    }
}

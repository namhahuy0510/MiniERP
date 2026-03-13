using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MiniERP.Controllers
{
    /// <summary>
    /// Hiển thị trang hướng dẫn sử dụng hệ thống.
    /// </summary>
    public class GuideController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Guide()
        {
            return View();
        }
    }
}


using Microsoft.AspNetCore.Mvc;

namespace MiniERP.Controllers
{
    public class SettingController : Controller
    {
        [HttpGet]
        public IActionResult Setting()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangeLanguage(string lang, string returnUrl)
        {
            // Thiết lập Cookie áp dụng cho toàn bộ domain (Path = "/")
            CookieOptions option = new CookieOptions 
            { 
                Expires = DateTime.Now.AddYears(1),
                Path = "/", // Rất quan trọng để các trang khác đều nhận được ngôn ngữ
                HttpOnly = true,
                Secure = true // Nếu chạy https
            };
            
            Response.Cookies.Append("lang", lang, option);

            // Kiểm tra returnUrl để tránh lỗi điều hướng
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Setting");
        }
    }
}
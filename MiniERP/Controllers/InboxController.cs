using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniERP.Models;

namespace MiniERP.Controllers
{
    [Authorize]
    public class InboxController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public InboxController(IWebHostEnvironment env)
        {
            _env = env;
        }

        private string GetAdminFolder()
        {
            var adminFolder = Path.Combine(_env.ContentRootPath, "Admin");
            if (!Directory.Exists(adminFolder))
            {
                Directory.CreateDirectory(adminFolder);
            }
            return adminFolder;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var notifications = new List<AdminNotification>();

            var adminFolder = GetAdminFolder();
            if (Directory.Exists(adminFolder))
            {
                var jsonFiles = Directory.GetFiles(adminFolder, "*.json", SearchOption.TopDirectoryOnly);
                foreach (var file in jsonFiles)
                {
                    try
                    {
                        var json = System.IO.File.ReadAllText(file);
                        // Cho phép file json có thể là 1 object hoặc 1 array
                        if (json.TrimStart().StartsWith("["))
                        {
                            var list = JsonSerializer.Deserialize<List<AdminNotification>>(json);
                            if (list != null) notifications.AddRange(list);
                        }
                        else
                        {
                            var item = JsonSerializer.Deserialize<AdminNotification>(json);
                            if (item != null) notifications.Add(item);
                        }
                    }
                    catch
                    {
                        // Bỏ qua file lỗi, tránh crash cả trang
                    }
                }
            }

            // Sắp xếp mới nhất lên đầu
            notifications = notifications
                .OrderByDescending(n => n.CreatedAt ?? DateTime.MinValue)
                .ToList();

            return View(notifications);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult CreateNotification(AdminNotification model)
        {
            if (string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.Message))
            {
                TempData["InboxError"] = "Tiêu đề và nội dung thông báo là bắt buộc.";
                return RedirectToAction(nameof(Index));
            }

            model.Id ??= Guid.NewGuid().ToString("N");
            model.CreatedAt ??= DateTime.Now;
            model.From ??= User.Identity?.Name ?? "Admin";

            var adminFolder = GetAdminFolder();
            var filePath = Path.Combine(adminFolder, "admin_notifications.json");

            var list = new List<AdminNotification>();
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    var existingJson = System.IO.File.ReadAllText(filePath);
                    if (!string.IsNullOrWhiteSpace(existingJson))
                    {
                        list = JsonSerializer.Deserialize<List<AdminNotification>>(existingJson) ?? new List<AdminNotification>();
                    }
                }
                catch
                {
                    list = new List<AdminNotification>();
                }
            }

            list.Add(model);

            var json = JsonSerializer.Serialize(list, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            System.IO.File.WriteAllText(filePath, json);

            TempData["InboxSuccess"] = "Đã gửi thông báo chung thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteNotification(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction(nameof(Index));
            }

            var adminFolder = GetAdminFolder();
            if (!Directory.Exists(adminFolder))
            {
                return RedirectToAction(nameof(Index));
            }

            var jsonFiles = Directory.GetFiles(adminFolder, "*.json", SearchOption.TopDirectoryOnly);
            foreach (var file in jsonFiles)
            {
                try
                {
                    var json = System.IO.File.ReadAllText(file);
                    if (string.IsNullOrWhiteSpace(json)) continue;

                    if (json.TrimStart().StartsWith("["))
                    {
                        var list = JsonSerializer.Deserialize<List<AdminNotification>>(json);
                        if (list == null) continue;

                        var beforeCount = list.Count;
                        list = list.Where(x => !string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase)).ToList();
                        if (list.Count != beforeCount)
                        {
                            // Có thay đổi, ghi lại file
                            var newJson = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
                            System.IO.File.WriteAllText(file, newJson);
                        }
                    }
                    else
                    {
                        var item = JsonSerializer.Deserialize<AdminNotification>(json);
                        if (item != null && string.Equals(item.Id, id, StringComparison.OrdinalIgnoreCase))
                        {
                            // Xóa hẳn file nếu là 1 object duy nhất
                            System.IO.File.Delete(file);
                        }
                    }
                }
                catch
                {
                    // Bỏ qua file lỗi
                }
            }

            TempData["InboxSuccess"] = "Đã xóa thông báo.";
            return RedirectToAction(nameof(Index));
        }
    }
}


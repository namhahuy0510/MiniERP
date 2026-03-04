using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace MiniERP.Services
{
    public interface IJsonLocalizationService
    {
        string GetString(string key);
    }

    public class JsonLocalizationService : IJsonLocalizationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _basePath;

        public JsonLocalizationService(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _basePath = Path.Combine(webHostEnvironment.ContentRootPath, "i18n");
        }

        public string GetString(string key)
        {
            var lang = _httpContextAccessor.HttpContext.Request.Cookies["lang"] ?? "vi";
            var filePath = Path.Combine(_basePath, $"{lang}.json");

            if (!File.Exists(filePath)) return key;

            try
            {
                var jsonString = File.ReadAllText(filePath);
                var translations = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
                return (translations != null && translations.TryGetValue(key, out var value)) ? value : key;
            }
            catch { return key; }
        }
    }
}
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Platform.Lib.Core.Services;
using System.Collections.Frozen;
using System.Text.Json;

namespace Platform.Lib.Infrastructure.Services.Localization
{
    // This service loads localization resources from JSON files located in the "Localization" folder of the application's content root path.
    // It provides methods to retrieve localized strings based on the current request's culture,
    // falling back to English if the specified culture is not found.
    public sealed class JsonLocalizationService : ILocalizationService
    {
        private readonly FrozenDictionary<string, FrozenDictionary<string, string>> _resources;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JsonLocalizationService(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _resources = LoadResources(environment.ContentRootPath);
            _httpContextAccessor = httpContextAccessor;
        }

        private static FrozenDictionary<string, FrozenDictionary<string, string>> LoadResources(string rootPath)
        {
            var localizationPath = Path.Combine(rootPath, "Localization");

            if (!Directory.Exists(localizationPath))
            {
                return FrozenDictionary<string, FrozenDictionary<string, string>>.Empty;
            }

            var resources = new Dictionary<string, FrozenDictionary<string, string>>();

            foreach (var file in Directory.EnumerateFiles(localizationPath, "*.json"))
            {
                var culture = Path.GetFileNameWithoutExtension(file);

                var json = File.ReadAllText(file);

                var values = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                             ?? new Dictionary<string, string>();

                resources[culture] = values.ToFrozenDictionary();
            }

            return resources.ToFrozenDictionary();
        }

        public string Get(string key)
        {
            var culture = _httpContextAccessor.HttpContext?.Request.Headers["Accept-Language"]
                .ToString()
                .Split(',')
                .FirstOrDefault()?
                .Split('-')
                .FirstOrDefault()
                ?? "en";

            if (!_resources.TryGetValue(culture, out var values))
            {
                _resources.TryGetValue("en", out values);
            }

            if (values != null && values.TryGetValue(key, out var value))
            {
                return value;
            }

            return key;
        }

        public string Get(string key, params object[] args)
        {
            return string.Format(Get(key), args);
        }

    }

}
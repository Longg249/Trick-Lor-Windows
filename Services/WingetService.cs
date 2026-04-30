using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TrickLor.Models;

namespace TrickLor.Services
{
    public static class WingetService
    {
        private static readonly HttpClient _http = new();
        private const string SearchApiUrl = "https://api.winget.run/v2/packages?query={0}&take=50";

        public static async Task<List<WingetPackage>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<WingetPackage>();

            var encoded = Uri.EscapeDataString(query.Trim());
            var url = string.Format(SearchApiUrl, encoded);

            string responseBody;
            try
            {
                responseBody = await _http.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể kết nối tới winget.run: {ex.Message}");
            }

            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                if (!root.TryGetProperty("packages", out var packagesElement))
                    throw new Exception("API không trả về trường 'packages'.");

                var packages = new List<WingetPackage>();
                foreach (var pkg in packagesElement.EnumerateArray())
                {
                    var package = new WingetPackage();

                    if (pkg.TryGetProperty("id", out var idProp))
                        package.Id = idProp.GetString() ?? "";
                    else
                        continue;

                    if (pkg.TryGetProperty("name", out var nameProp))
                        package.Name = nameProp.GetString() ?? package.Id;
                    else
                        package.Name = package.Id;

                    if (pkg.TryGetProperty("latestVersion", out var verProp))
                        package.Version = verProp.GetString() ?? "";
                    if (pkg.TryGetProperty("publisher", out var pubProp))
                        package.Publisher = pubProp.GetString() ?? "";
                    if (pkg.TryGetProperty("description", out var descProp))
                        package.Description = descProp.GetString() ?? "";

                    packages.Add(package);
                }

                return packages;
            }
            catch (JsonException ex)
            {
                throw new Exception($"Dữ liệu từ API không đúng định dạng JSON: {ex.Message}");
            }
        }
    }
}
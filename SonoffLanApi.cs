using System.Net.Http.Json;
using System.Text.Json;

namespace SonoffWizardV2
{
    public static class SonoffLanApi
    {
        private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(2) };

        /// <summary>POST /zeroconf/info and return deviceid, or null on failure.</summary>
        public static async Task<string?> GetDeviceIdAsync(string ip, CancellationToken ct = default)
        {
            var body = new { deviceid = "", data = new { } };

            try
            {
                using var resp = await _http.PostAsJsonAsync($"http://{ip}:8081/zeroconf/info", body, ct);
                if (!resp.IsSuccessStatusCode) return null;

                var json = await resp.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: ct);
                return json?.RootElement.GetProperty("deviceid").GetString();   // ← from root
            }
            catch
            {
                return null;
            }
        }
    }
}

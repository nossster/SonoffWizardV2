using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Sonoff.Core
{
    public class SonoffClient
    {
        private readonly HttpClient _http = new();
        private readonly string _baseUrl;

        public SonoffClient(string hostPort)
        {
            _baseUrl = $"http://{hostPort}";
        }

        /// <summary>
        /// Turn device on or off.
        /// </summary>
        /// <returns>true if the HTTP request succeeded.</returns>
        public bool Switch(string deviceId, bool turnOn)
        {
            var body = new
            {
                deviceid = deviceId,
                data = new { @switch = turnOn ? "on" : "off" }
            };

            string json = JsonSerializer.Serialize(body);
            var response = _http.PostAsync(
                               $"{_baseUrl}/zeroconf/switch",
                               new StringContent(json, Encoding.UTF8, "application/json"))
                             .Result;

            return response.IsSuccessStatusCode;
        }
    }
}

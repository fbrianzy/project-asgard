using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherDashboard.Models;

namespace WeatherDashboard.Services
{
    public class OpenWeatherService
    {
        private readonly HttpClient _http = new();
        public async Task<WeatherResponse?> FetchAsync(string city, string apiKey)
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={Uri.EscapeDataString(city)}&appid={apiKey}&units=metric";
            using var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<WeatherResponse>(json);
        }

        public string IconUrl(string iconCode) => $"https://openweathermap.org/img/wn/{iconCode}@2x.png";
    }
}

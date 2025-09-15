using System.Text.Json.Serialization;

namespace WeatherDashboard.Models
{
    public class WeatherResponse
    {
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("weather")] public List<WeatherDesc>? Weather { get; set; }
        [JsonPropertyName("main")] public MainInfo? Main { get; set; }
        [JsonPropertyName("wind")] public WindInfo? Wind { get; set; }
        [JsonPropertyName("dt")] public long Dt { get; set; }
        [JsonPropertyName("sys")] public SysInfo? Sys { get; set; }
    }

    public class WeatherDesc
    {
        [JsonPropertyName("main")] public string? Main { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("icon")] public string? Icon { get; set; }
    }

    public class MainInfo
    {
        [JsonPropertyName("temp")] public double Temp { get; set; }
        [JsonPropertyName("humidity")] public int Humidity { get; set; }
        [JsonPropertyName("pressure")] public int Pressure { get; set; }
        [JsonPropertyName("temp_min")] public double TempMin { get; set; }
        [JsonPropertyName("temp_max")] public double TempMax { get; set; }
    }

    public class WindInfo
    {
        [JsonPropertyName("speed")] public double Speed { get; set; }
    }

    public class SysInfo
    {
        [JsonPropertyName("country")] public string? Country { get; set; }
    }
}

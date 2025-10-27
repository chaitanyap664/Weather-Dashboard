namespace WeatherDashboardAPI.Models
{
    public class WeatherSettings
    {
        public string APIBaseURL { get; set; } = string.Empty;
        public string WeatherApiKey { get; set; } = string.Empty;
        public int CacheSeconds { get; set; } = 600;
    }
}

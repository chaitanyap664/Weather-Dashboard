namespace WeatherDashboardAPI.Models
{
      /// <summary>
    /// Represents the configuration settings used by the Weather Dashboard API.
    /// </summary>
    /// <remarks>
    /// This class binds to the <c>WeatherSettings</c> section of <c>appsettings.json</c> or 
    /// user secrets to provide external configuration values such as API base URL,
    /// authentication key, and cache duration.
    /// </remarks>
    public class WeatherSettings
    {
        public string APIBaseURL { get; set; } = string.Empty;
        public string WeatherApiKey { get; set; } = string.Empty;
        public int CacheSeconds { get; set; } = 600;
    }
}

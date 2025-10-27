namespace WeatherDashboardAPI.DTO
{
   /// <summary>
/// Represents a standardized response object for weather API data.
/// </summary>
/// <remarks>
/// This Data Transfer Object (DTO) is used to encapsulate and return clean,  
/// frontend-friendly weather data from external APIs without exposing internal models.
/// </remarks>
    public class WeatherResponseDto
    {
        public string City { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;

        public List<ForecastDto> Forecast { get; set; } = new();
        public List<HourlyDto> Hourly { get; set; } = new();
    }

    /// <summary>
    /// Represents a single forecast entry (e.g., next 4 days).
    /// </summary>
    public class ForecastDto
    {
        public string Date { get; set; } = string.Empty;
        public double Temp { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a single hourly forecast entry.
    /// </summary>
    public class HourlyDto
    {
        public string Time { get; set; } = string.Empty;
        public double Temp { get; set; }
        public string Icon { get; set; } = string.Empty;
    }
}
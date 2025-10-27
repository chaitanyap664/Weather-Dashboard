using System.Collections.Generic;

namespace WeatherDashboardAPI.Models
{
    /// <summary>
/// Represents the internal domain model for storing current, forecast, and hourly weather data.
/// </summary>
/// <remarks>
/// This class serves as the core structure used within the API service logic before  
/// mapping data to frontend DTOs. It ensures strong typing and clear separation  
/// between internal models and external API responses.
/// </remarks>

    public class WeatherInfo
    {
        // Current weather
        public string? City { get; set; }
        public double Temperature { get; set; }
        public string? Description { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
        public string? Icon { get; set; }

        // Forecast (next 4 days)
        public List<ForecastInfo>? Forecast { get; set; }

        // Hourly forecast (next 12 hours)
        public List<HourlyInfo>? Hourly { get; set; }
    }
    /// <summary>
/// Represents forecast information for a specific day.
/// </summary>
    public class ForecastInfo
    {
        public string? Date { get; set; }
        public double Temp { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
    }
/// <summary>
/// Represents hourly weather data including temperature and icon.
/// </summary>
    public class HourlyInfo
    {
        public string? Time { get; set; }
        public double Temp { get; set; }
        public string? Icon { get; set; }
    }
}

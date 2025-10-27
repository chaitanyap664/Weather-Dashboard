using WeatherDashboardAPI.Models;

namespace WeatherDashboardAPI.Interfaces
{
  /// <summary>
/// Defines a contract for retrieving and managing weather information.
/// </summary>
/// <remarks>
/// Any implementation of this interface must provide asynchronous methods for fetching
/// weather data, including current conditions, hourly forecasts, and extended daily forecasts.
/// </remarks>


    public interface IWeatherService
    {

        Task<WeatherInfo?> GetWeatherAsync(string city, int days, CancellationToken ct = default);
        Task<string?> GetDefaultCityAsync();
        Task<(bool Success, string Message)> SetDefaultCityAsync(string city, CancellationToken ct = default);

    }
}

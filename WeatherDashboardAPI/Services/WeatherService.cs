using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using WeatherDashboardAPI.Models;
using WeatherDashboardAPI.Interfaces;
using WeatherDashboardAPI.DTO;

/// <summary>
/// Provides the core business logic for fetching and transforming weather data.
/// </summary>
/// <remarks>
/// The <see cref="WeatherService"/> communicates with external weather APIs (e.g., OpenWeatherMap or WeatherAPI),
/// applies retry and caching policies, and maps API responses into internal domain models.  
/// It ensures resilient and optimized communication with the external provider.
/// </remarks>

namespace WeatherDashboardAPI.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<WeatherService> _logger;
        private readonly TimeSpan _cacheDuration;

        private readonly WeatherSettings _settings;
        private const string DefaultCityKey = "DefaultCity";
        public WeatherService(HttpClient httpClient, IMemoryCache cache, ILogger<WeatherService> logger, IOptions<WeatherSettings> options, TimeSpan? cacheDuration = null)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
            _settings = options.Value;
            _cacheDuration = cacheDuration ?? TimeSpan.FromMinutes(5);
        }
        /// <summary>Fetches weather details for a given city and forecast period.</summary>
        public async Task<WeatherInfo?> GetWeatherAsync(string city, int days, CancellationToken ct = default)
        {
            string cacheWeatherKey = $"weather_{city.Trim().ToLowerInvariant()}_{days}";
            if (_cache.TryGetValue(cacheWeatherKey, out WeatherInfo? cachedWeather))
                return cachedWeather;
            var url = $"{_settings.APIBaseURL}?key={_settings.WeatherApiKey}&q={Uri.EscapeDataString(city)}&days={days}&aqi=no&alerts=no";
            try
            {
                var response = await _httpClient.GetAsync(url, ct);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Weather API failed for {City} with {Code}", city, response.StatusCode);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync(ct);
                var root = JsonDocument.Parse(json).RootElement;

                if (root.TryGetProperty("error", out var err))
                {
                    _logger.LogWarning("Weather API error for {City}: {Msg}", city, err.GetProperty("message").GetString());
                    return null;
                }
                //var location = root.GetProperty("location");
                var locationName = root.GetProperty("location").GetProperty("name").GetString();
                if (!string.Equals(locationName, city, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Rejected fuzzy match for {InputCity} resolved to {ResolvedCity}", city, locationName);
                    return null;
                }
                if (root.ValueKind != JsonValueKind.Object ||
                      !root.TryGetProperty("location", out var location) ||
                      !root.TryGetProperty("current", out var current))
                {
                    _logger.LogWarning("Malformed or incomplete JSON response for {City}", city);
                    return null;
                }

                //Deserialize into a strongly typed object
                var apiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse == null || apiResponse.Location == null || apiResponse.Current == null)
                    return null;

                //Map to your internal WeatherInfo model
                var weather = new WeatherInfo
                {
                    City = apiResponse.Location.Name,
                    Temperature = apiResponse.Current.TempC,
                    Description = apiResponse.Current.Condition?.Text ?? "",
                    Humidity = apiResponse.Current.Humidity,
                    WindSpeed = apiResponse.Current.WindKph,
                    Icon = "https:" + (apiResponse.Current.Condition?.Icon ?? ""),
                    Forecast = apiResponse.Forecast?.ForecastDay?.Skip(1)
                        .Select(f => new ForecastInfo
                        {
                            Date = f.Date,
                            Temp = f.Day?.AvgTempC ?? 0,
                            Description = f.Day?.Condition?.Text ?? "",
                            Icon = "https:" + (f.Day?.Condition?.Icon ?? "")
                        }).ToList() ?? new List<ForecastInfo>(),
                    Hourly = apiResponse.Forecast?.ForecastDay?.FirstOrDefault()?.Hour?
                        .Take(12)
                        .Select(h => new HourlyInfo
                        {
                            Time = h.Time?.Split(' ')[1],
                            Temp = h.TempC,
                            Icon = "https:" + (h.Condition?.Icon ?? "")
                        }).ToList() ?? new List<HourlyInfo>()
                };

                _cache.Set(cacheWeatherKey, weather, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration,
                    SlidingExpiration = TimeSpan.FromMinutes(2),
                    Priority = CacheItemPriority.Normal
                });
                return weather;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning(ex, "Request to weather API for {City} timed out.", city);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching weather for {City}", city);
                return null;
            }

        }
        
        /// <summary>Retrieves the default city from the in-memory cache.</summary>
        public Task<string?> GetDefaultCityAsync()
        {
            _cache.TryGetValue(DefaultCityKey, out string? city);
            return Task.FromResult(city);
        }
        
        /// <summary>Validates and saves the default city into cache.</summary>
        public async Task<(bool Success, string Message)> SetDefaultCityAsync(string city, CancellationToken ct = default)
        {
            var validCity = await GetWeatherAsync(city, 1, ct);
            if (validCity == null)
                return (false, $"City '{city}' is not valid.");
            _cache.Set(DefaultCityKey, city, TimeSpan.FromDays(365));
            _logger.LogInformation("Default city set to {City}", city);
            return (true, $"'{city}' has been set as the default city.");
        }
    }
}

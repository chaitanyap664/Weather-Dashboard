using System.Text.Json.Serialization;

namespace WeatherDashboardAPI.DTO
{
    /// <summary>
    /// Represents the root object returned by the external Weather API.
    /// </summary>
    /// <remarks>
    /// This DTO is used to deserialize raw JSON responses from the Weather API before mapping 
    /// them into the internal <see cref="Models.WeatherInfo"/> domain model.
    /// </remarks>
    public class WeatherApiResponse
    {
        [JsonPropertyName("location")]
        public LocationInfo? Location { get; set; }

        [JsonPropertyName("current")]
        public CurrentWeather? Current { get; set; }

        [JsonPropertyName("forecast")]
        public ForecastRoot? Forecast { get; set; }
    }

    public class LocationInfo
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class CurrentWeather
    {
        [JsonPropertyName("temp_c")]
        public double TempC { get; set; }

        [JsonPropertyName("humidity")]
        public double Humidity { get; set; }

        [JsonPropertyName("wind_kph")]
        public double WindKph { get; set; }

        [JsonPropertyName("condition")]
        public Condition? Condition { get; set; }
    }

    public class Condition
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }
    }

    public class ForecastRoot
    {
        [JsonPropertyName("forecastday")]
        public List<ForecastDay>? ForecastDay { get; set; }
    }

    public class ForecastDay
    {
        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("day")]
        public DayInfo? Day { get; set; }

        [JsonPropertyName("hour")]
        public List<HourInfo>? Hour { get; set; }
    }

    public class DayInfo
    {
        [JsonPropertyName("avgtemp_c")]
        public double AvgTempC { get; set; }

        [JsonPropertyName("condition")]
        public Condition? Condition { get; set; }
    }

    public class HourInfo
    {
        [JsonPropertyName("time")]
        public string? Time { get; set; }

        [JsonPropertyName("temp_c")]
        public double TempC { get; set; }

        [JsonPropertyName("condition")]
        public Condition? Condition { get; set; }
    }
}

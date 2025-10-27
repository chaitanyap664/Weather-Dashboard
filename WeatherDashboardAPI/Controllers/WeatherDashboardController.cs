using Microsoft.AspNetCore.Mvc;
using WeatherDashboardAPI.Interfaces;
using WeatherDashboardAPI.DTO;
using AutoMapper;
using Polly.Timeout;

namespace WeatherDashboardAPI.Controllers
{
    /// <summary>
    /// Handles incoming HTTP requests related to weather information.
    /// </summary>
    /// <remarks>
    /// The <see cref="WeatherDashboardController"/> acts as the entry point for clients (e.g., the React frontend)  
    /// to request weather data by city, manage default locations, and retrieve forecasts.  
    /// It delegates all business logic to the <see cref="IWeatherService"/> implementation and  
    /// ensures consistent HTTP responses (200, 400, 404, 500).
    /// </remarks>

    [ApiController]
    [Route("[controller]")]
    public class WeatherDashboardController : ControllerBase
    {
        private readonly IWeatherService _service;
        private readonly ILogger<WeatherDashboardController> _logger;
        private readonly IMapper _mapper;

        public WeatherDashboardController(IWeatherService service, IMapper mapper, ILogger<WeatherDashboardController> logger)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves weather details for the specified city.
        /// </summary>
        /// <param name="city">City name to fetch weather data for.</param>
        /// <param name="days">Number of forecast days (default is 5).</param>
        /// <returns>Weather information for the specified city.</returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string city, [FromQuery] int days = 5)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest(new { message = "City parameter is required." });

            try
            {
                var result = await _service.GetWeatherAsync(city, days);
                if (result == null)
                    return NotFound(new { message = $"City '{city}' not found or data unavailable." });

                //Map domain model to DTO
                var dto = _mapper.Map<WeatherResponseDto>(result);
                return Ok(dto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid city request: {City}", city);
                return BadRequest(new { message = ex.Message });
            }
            catch (TimeoutRejectedException ex)
            {
                _logger.LogWarning(ex, "Weather API timed out for city {City}", city);
                return StatusCode(504, new { message = "Weather service timed out, please try again." });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error while fetching weather for {City}", city);
                return StatusCode(503, new { message = "Weather service temporarily unavailable. Please try again shortly." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching weather for {City}", city);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// POST /weatherdashboard/default { city: "London" }
        /// </summary>
        [HttpPut("default")]
        public async Task<IActionResult> SetDefault([FromBody] DefaultCityModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.City))
                return BadRequest(new { message = "City is required." });

            try
            {
                var (success, message) = await _service.SetDefaultCityAsync(model.City);

                if (!success)
                {
                    _logger.LogWarning("Invalid default city set attempt: {City}", model.City);
                    return BadRequest(new { message });
                }

                _logger.LogInformation("Default city successfully set to {City}", model.City);
                return Ok(new { city = model.City, message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting default city: {City}", model.City);
                return StatusCode(500, new { message = "Unexpected error occurred." });
            }
        }

        /// <summary>
        /// GET /weatherdashboard/default
        /// </summary>
        [HttpGet("default")]
        public async Task<IActionResult> GetDefault()
        {
            try
            {
                var city = await _service.GetDefaultCityAsync();
                return Ok(new { city });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error getting default city");
                return StatusCode(500, new { message = "Unexpected error occurred." });
                
            }
        }
    }

    public class DefaultCityModel
    {
        public string City { get; set; } = string.Empty;
    }
}

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using WeatherDashboardAPI.Controllers;
using WeatherDashboardAPI.DTO;
using WeatherDashboardAPI.Interfaces;
using WeatherDashboardAPI.Models;
using WeatherDashboardAPI.Services;

namespace WeatherDashboardAPI.Tests
{
     /// <summary>
    /// Unit tests for <see cref="WeatherDashboardController"/> verifying endpoint behaviors,
    /// response codes, and integration with <see cref="IWeatherService"/>.
    /// </summary>
    [TestFixture]
    public class WeatherDashboardControllerTests
    {
        private Mock<IWeatherService> _mockService;
        private Mock<IMapper> _mockMapper;
        private WeatherDashboardController _controller;
         /// <summary>
        /// Initializes the controller and its mock dependencies before each test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IWeatherService>();
            _mockMapper = new Mock<IMapper>();
            _controller = new WeatherDashboardController(
                _mockService.Object,
                _mockMapper.Object,
                NullLogger<WeatherDashboardController>.Instance);
        }
        
        /// <summary>
        /// Verifies that <see cref="WeatherDashboardController.Get"/> returns
        /// <see cref="OkObjectResult"/> (HTTP 200) when a valid city is found.
        /// </summary>
        [Test]
        public async Task GetWeather_ReturnsOk_WhenCityFound()
        {
            var city = "London";
            // Arrange
            var weather = new WeatherInfo
            {
                City = city,
                Temperature = 20,
                Description = "Sunny",
                Humidity = 50,
                WindSpeed = 5
            };

            _mockService.Setup(s => s.GetWeatherAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(weather);
            _mockMapper.Setup(m => m.Map<WeatherResponseDto>(It.IsAny<WeatherInfo>()))
              .Returns(new WeatherResponseDto
              {
                  City = "London",
                  Temperature = 20,
                  Description = "Sunny"
              });
            // Act
            var result = await _controller.Get("London");

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.Not.Null);
            Assert.That(((WeatherResponseDto)okResult.Value!).City, Is.EqualTo(city));
        }
            /// <summary>
        /// Ensures <see cref="WeatherDashboardController.Get"/> returns
        /// <see cref="NotFoundObjectResult"/> (HTTP 404) for an invalid city.
        /// </summary>
        [Test]
        public async Task GetWeather_ReturnsNotFound_WhenCityInvalid()
        {
            // Arrange: simulate invalid city
            _mockService
                .Setup(s => s.GetWeatherAsync("InvalidCity", 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((WeatherInfo?)null);

            // Act
            var result = await _controller.Get("InvalidCity");

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>(),
                "Should return 404 NotFound for invalid city.");

            var notFound = result as NotFoundObjectResult;
            Assert.That(notFound?.Value?.ToString(), Does.Contain("not found"));
        }
        /// <summary>
        /// Ensures <see cref="WeatherDashboardController.Get"/> returns
        /// <see cref="BadRequestObjectResult"/> (HTTP 400) when the city parameter is empty.
        /// </summary>
        [Test]
        public async Task GetWeather_ReturnsBadRequest_WhenCityIsEmpty()
        {
            // Act
            var result = await _controller.Get("");

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>(),
                "Should return 400 BadRequest when no city is provided.");
        }
         /// <summary>
        /// Verifies that the underlying <see cref="WeatherService.GetWeatherAsync"/> 
        /// handles <see cref="TaskCanceledException"/> (timeouts) gracefully without throwing.
        /// </summary>
        [Test]
        public async Task GetWeatherAsync_ReturnsNull_OnTimeout()
        {
            // Arrange
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new TaskCanceledException()); // simulate timeout

            var httpClient = new HttpClient(handler.Object);
            var cache = new MemoryCache(new MemoryCacheOptions());
            var options = Options.Create(new WeatherSettings
            {
                WeatherApiKey = "fake-key",
                APIBaseURL = "http://fake-api.com"
            });
            var service = new WeatherService(httpClient, cache, new NullLogger<WeatherService>(), options);

            // Act
            var result = await service.GetWeatherAsync("London", 1);

            // Assert
            Assert.That(result, Is.Null, "Expected null on timeout, not exception.");
        }


    }
}
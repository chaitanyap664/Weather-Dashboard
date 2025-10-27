using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using WeatherDashboardAPI.Controllers;
using WeatherDashboardAPI.Interfaces;
using WeatherDashboardAPI.Models;

namespace WeatherDashboardAPI.Tests
{
    [TestFixture]
    public class WeatherControllerTests
    {
        private Mock<IWeatherService> _mockService = null!;
        private Mock<ILogger<WeatherController>> _mockLogger = null!;
        private WeatherController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IWeatherService>();
            _mockLogger = new Mock<ILogger<WeatherController>>();
            _controller = new WeatherController(_mockService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetWeather_ReturnsOk_WhenCityFound()
        {
            // Arrange
            var weather = new WeatherInfo
            {
                City = "London",
                Temperature = 20,
                Description = "Sunny",
                Humidity = 50,
                WindSpeed = 5
            };

            _mockService
                .Setup(s => s.GetWeatherAsync("London", It.IsAny<int>()))
                .ReturnsAsync(weather);

            // Act
            var result = await _controller.Get("London");

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.Not.Null);
        }

        [Test]
        public async Task GetWeather_ReturnsNotFound_WhenCityInvalid()
        {
            // Arrange: simulate invalid city
            _mockService
                .Setup(s => s.GetWeatherAsync("InvalidCity", It.IsAny<int>()))
                .ReturnsAsync((WeatherInfo?)null);

            // Act
            var result = await _controller.Get("InvalidCity");

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>(), 
                "Should return 404 NotFound for invalid city.");

            var notFound = result as NotFoundObjectResult;
            Assert.That(notFound?.Value?.ToString(), Does.Contain("not found"));
        }

        [Test]
        public async Task GetWeather_ReturnsBadRequest_WhenCityIsEmpty()
        {
            // Act
            var result = await _controller.Get("");

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>(),
                "Should return 400 BadRequest when no city is provided.");
        }
    }
}

using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using WeatherDashboardAPI.Models;
using WeatherDashboardAPI.Services;
using System.Text.Json;
using WeatherDashboardAPI.DTO;

namespace WeatherDashboardAPI.Tests
{
    [TestFixture]
    public class WeatherServiceTests
    {
        private IMemoryCache _cache = null!;
        private IOptions<WeatherSettings> _options = null!;

        [SetUp]
        public void Setup()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _options = Options.Create(new WeatherSettings
            {
                WeatherApiKey = "fake-key",
                APIBaseURL = "http://api.weatherapi.com/v1/"
            });
        }

        [TearDown]
        public void Cleanup()
        {
            _cache.Dispose();
        }

        private static HttpClient CreateMockHttpClient(string jsonResponse, HttpStatusCode code = HttpStatusCode.OK)
        {
            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = code,
                    Content = new StringContent(jsonResponse)
                });

            return new HttpClient(handler.Object);
        }

        [Test]
        public async Task GetWeatherAsync_ReturnsParsedWeather_WhenApiSucceeds()
        {
            string fakeJson = @"{
                ""location"": { ""name"": ""London"" },
                ""current"": {
                    ""temp_c"": 20,
                    ""humidity"": 65,
                    ""wind_kph"": 10,
                    ""condition"": { ""text"": ""Cloudy"", ""icon"": ""//cdn.weatherapi.com/icon.png"" }
                },
                ""forecast"": {
                    ""forecastday"": [
                        {
                            ""date"": ""2025-10-25"",
                            ""day"": { 
                                ""avgtemp_c"": 21, 
                                ""condition"": { ""text"": ""Sunny"", ""icon"": ""//cdn.weatherapi.com/sunny.png"" }
                            },
                            ""hour"": [
                                { ""time"": ""2025-10-25 00:00"", ""temp_c"": 18 },
                                { ""time"": ""2025-10-25 01:00"", ""temp_c"": 17 }
                            ]
                        }
                    ]
                }
            }";

            var httpClient = CreateMockHttpClient(fakeJson);
            var service = new WeatherService(httpClient, _cache, new NullLogger<WeatherService>(), _options);

            var result = await service.GetWeatherAsync("London", 1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.City, Is.EqualTo("London"));
            Assert.That(result.Temperature, Is.EqualTo(20));
            Assert.That(result.Description, Does.Contain("Cloudy"));
        }

        [Test]
        public async Task SetDefaultCityAsync_SavesToCache_IfValidCity()
        {
            string fakeJson = @"{
                ""location"": { ""name"": ""Paris"" },
                ""current"": { ""temp_c"": 22, ""humidity"": 50, ""wind_kph"": 15, 
                               ""condition"": { ""text"": ""Sunny"", ""icon"": ""icon.png"" } }
            }";

            var httpClient = CreateMockHttpClient(fakeJson);
            var service = new WeatherService(httpClient, _cache, new NullLogger<WeatherService>(), _options);

            var (success, message) = await service.SetDefaultCityAsync("Paris");

            Assert.That(success, Is.True);
            Assert.That(message, Does.Contain("Paris"));
            var cached = await service.GetDefaultCityAsync();
            Assert.That(cached, Is.EqualTo("Paris"));
        }

        [Test]
        public async Task SetDefaultCityAsync_ReturnsValidationMessage_IfCityInvalid()
        {
            string fuzzyJson = @"{
                ""location"": { ""name"": ""Dhaka"" },
                ""current"": { ""temp_c"": 30, ""humidity"": 80, ""wind_kph"": 10,
                               ""condition"": { ""text"": ""Cloudy"", ""icon"": ""icon.png"" } }
            }";

            var httpClient = CreateMockHttpClient(fuzzyJson);
            var service = new WeatherService(httpClient, _cache, new NullLogger<WeatherService>(), _options);

            var (success, message) = await service.SetDefaultCityAsync("D");

            Assert.That(success, Is.False);
            Assert.That(message, Does.Contain("not valid").Or.Contain("does not exactly match"));
            var cached = await service.GetDefaultCityAsync();
            Assert.That(cached, Is.Null.Or.Empty);
        }

        [Test]
        public async Task GetDefaultCityAsync_ReturnsNull_WhenNotSet()
        {
            var httpClient = CreateMockHttpClient("{}", HttpStatusCode.OK);
            var service = new WeatherService(httpClient, _cache, new NullLogger<WeatherService>(), _options);

            var city = await service.GetDefaultCityAsync();

            Assert.That(city, Is.Null);
        }

        [Test]
        public async Task GetWeatherAsync_UsesCache_OnSecondCall()
        {
            // Arrange
            string fakeJson = @"{
                ""location"": { ""name"": ""London"" },
                ""current"": { 
                    ""temp_c"": 20, 
                    ""humidity"": 50, 
                    ""wind_kph"": 15, 
                    ""condition"": { ""text"": ""Cloudy"", ""icon"": ""//cdn.weatherapi.com/icon.png"" } 
                },
                ""forecast"": { ""forecastday"": [] }
            }";

            int apiCallCount = 0;

            // Mock HttpMessageHandler to count API calls
            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(() =>
                {
                    apiCallCount++;
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(fakeJson)
                    };
                });

            var httpClient = new HttpClient(handler.Object);
            var service = new WeatherService(httpClient, _cache, new NullLogger<WeatherService>(), _options);

            // Act — first call (should call API)
            var result1 = await service.GetWeatherAsync("London", 1);
            Assert.That(result1, Is.Not.Null);
            Assert.That(result1!.City, Is.EqualTo("London"));
            Assert.That(apiCallCount, Is.EqualTo(1), "First call should hit API once");

            // Act — second call (should hit cache)
            var result2 = await service.GetWeatherAsync("London", 1);

            // Assert
            Assert.That(result2, Is.Not.Null);
            Assert.That(result2!.City, Is.EqualTo("London"));
            Assert.That(apiCallCount, Is.EqualTo(1), "Second call should not call API again — cache hit");

            // Extra: confirm same reference if desired
            Assert.That(ReferenceEquals(result1, result2), Is.True, "Cached result should be same object instance");
        }
        [Test]
        public async Task GetWeatherAsync_CallsApiAgain_AfterCacheExpires()
        {
            // Arrange
            string fakeJson = @"{
        ""location"": { ""name"": ""Paris"" },
        ""current"": { 
            ""temp_c"": 25, 
            ""humidity"": 40, 
            ""wind_kph"": 12, 
            ""condition"": { ""text"": ""Sunny"", ""icon"": ""//cdn.weatherapi.com/sunny.png"" } 
        },
        ""forecast"": { ""forecastday"": [] }
    }";

            int apiCallCount = 0;

            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(() =>
                {
                    apiCallCount++;
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(fakeJson)
                    };
                });

            var httpClient = new HttpClient(handler.Object);
            var cache = new MemoryCache(new MemoryCacheOptions());
            var options = Options.Create(new WeatherSettings
            {
                WeatherApiKey = "fake-key",
                APIBaseURL = "http://fake-api.com"
            });

            // inject short TTL of 2 seconds
            var service = new WeatherService(httpClient, cache, new NullLogger<WeatherService>(), options, TimeSpan.FromSeconds(2));

            // Act — first call (cache miss)
            var result1 = await service.GetWeatherAsync("Paris", 1);
            Assert.That(apiCallCount, Is.EqualTo(1), "First call should trigger API request");

            // Act — second call (cache hit)
            var result2 = await service.GetWeatherAsync("Paris", 1);
            Assert.That(apiCallCount, Is.EqualTo(1), "Second call should use cache");

            // Wait for cache to expire
            await Task.Delay(TimeSpan.FromSeconds(3));

            // Act — third call (cache expired)
            var result3 = await service.GetWeatherAsync("Paris", 1);

            // Assert
            Assert.That(apiCallCount, Is.EqualTo(2), "After cache expiry, API should be called again");
        }
        [Test]
        public async Task GetWeatherAsync_ReturnsNull_WhenCityNotFound()
        {
            // Arrange
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("{\"error\":\"City not found\"}")
                });

            var httpClient = new HttpClient(handler.Object);
            var cache = new MemoryCache(new MemoryCacheOptions());
            var options = Options.Create(new WeatherSettings
            {
                WeatherApiKey = "fake-key",
                APIBaseURL = "http://fake-api.com"
            });

            var service = new WeatherService(httpClient, cache, new NullLogger<WeatherService>(), options);

            // Act
            var result = await service.GetWeatherAsync("InvalidCity", 1);

            // Assert
            Assert.That(result, Is.Null, "Invalid city should return null");
        }
        [Test]
        public void Deserialize_WeatherApiResponse_WorksCorrectly()
        {
            var basePath = AppContext.BaseDirectory;
            var filePath = Path.Combine(basePath, "Samples", "weather_sample.json");

            Assert.That(File.Exists(filePath), $"Sample file not found: {filePath}");
            var json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<WeatherApiResponse>(json);

            Assert.That(data, Is.Not.Null);
            Assert.That(data.Location?.Name, Is.EqualTo("London"));
            Assert.That(data.Current?.TempC, Is.EqualTo(18));
        }
        [Test]
        public async Task GetWeatherAsync_ReturnsNull_WhenProviderErrorFieldPresent()
        {
            var json = @"{ ""error"": { ""message"": ""Invalid API key"" } }";
            var http = CreateMockHttpClient(json, HttpStatusCode.OK);
            var svc = new WeatherService(http, _cache, new NullLogger<WeatherService>(), _options);

            var result = await svc.GetWeatherAsync("London", 1);

            Assert.That(result, Is.Null);
        }
        [Test]
        public async Task GetWeatherAsync_ReturnsNull_OnTimeout()
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(10)); // exceed service timeout
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(@"{}")
                    };
                });

            var http = new HttpClient(handler.Object);
            var svc = new WeatherService(http, _cache, new NullLogger<WeatherService>(), _options);

            var result = await svc.GetWeatherAsync("London", 1);

            Assert.That(result, Is.Null);
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using WeatherDashboardAPI.Interfaces;
using WeatherDashboardAPI.Services;
using WeatherDashboardAPI.Models;
using Polly.Extensions.Http;
using Polly;
using System.Net;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//Register memory cache
builder.Services.AddMemoryCache();

//Configure WeatherSettings
builder.Services.Configure<WeatherSettings>(
    builder.Configuration.GetSection("WeatherSettings"));

//Register typed HttpClient for WeatherService with Polly policies
builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
{
    client.BaseAddress = new Uri("https://api.weatherapi.com/");
    client.Timeout = Timeout.InfiniteTimeSpan; // important so Polly controls timeout
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(5)); // 5s timeout

//Add controllers with model validation
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
            new BadRequestObjectResult(context.ModelState);
    });

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Weather Dashboard API",
        Version = "v1",
        Description = "API for fetching and caching weather information."
    });
});
builder.Services.AddAutoMapper(typeof(Program));

//CORS configuration
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                     ?? new[] { "http://localhost:5173", "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Retry policy helper
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() // network failures or 5xx
        .OrResult(r => r.StatusCode == HttpStatusCode.RequestTimeout
                    || (int)r.StatusCode >= 500)
        .WaitAndRetryAsync(
            2,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            (outcome, timespan, retryAttempt, context) =>
            {
                Console.WriteLine(
                    $"Retry {retryAttempt} after {timespan.TotalSeconds}s due to " +
                    $"{outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
            });
}

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
      app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather Dashboard API v1");
        // o.RoutePrefix = string.Empty; // optional: serve UI at "/"
    });
}
// app.UseHttpsRedirection();  // Disabled for local demo simplicity
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();

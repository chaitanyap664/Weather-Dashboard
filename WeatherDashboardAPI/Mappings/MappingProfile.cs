using AutoMapper;
using WeatherDashboardAPI.Models;
using WeatherDashboardAPI.DTO;

/// <summary>
/// Defines the AutoMapper configuration used to map between
/// domain models (<see cref="WeatherInfo"/>, <see cref="ForecastInfo"/>, <see cref="HourlyInfo"/>)
/// and their corresponding Data Transfer Objects (DTOs).
/// </summary>
/// <remarks>
/// This profile simplifies model-to-DTO conversions for controller responses,
/// ensuring a clear separation between internal entities and API-facing structures.
/// </remarks>
public class MappingProfile : Profile
{
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class
        /// and configures mapping rules between internal models and DTOs.
        /// </summary>
    public MappingProfile()
    {

        /// <summary>
        /// Maps weather information from the <see cref="WeatherInfo"/> model
        /// to the <see cref="WeatherResponseDto"/> used in API responses.
        /// </summary>
        CreateMap<WeatherInfo, WeatherResponseDto>();

        /// <summary>
        /// Maps daily forecast data from <see cref="ForecastInfo"/> 
        /// to <see cref="ForecastDto"/>.
        /// </summary>
        CreateMap<ForecastInfo, ForecastDto>();

        /// <summary>
        /// Maps hourly forecast data from <see cref="HourlyInfo"/> 
        /// to <see cref="HourlyDto"/>.
        /// </summary>
        CreateMap<HourlyInfo, HourlyDto>();
    }
}
using AutoMapper;
using WeatherDashboardAPI.Models;
using WeatherDashboardAPI.DTO;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<WeatherInfo, WeatherResponseDto>();
        CreateMap<ForecastInfo, ForecastDto>();
        CreateMap<HourlyInfo, HourlyDto>();
    }
}
import { renderHook, act } from "@testing-library/react";
import { WeatherProvider } from "../context/WeatherContext";
import { useWeatherDashboard } from "../hooks/useWeatherDashboard";
import * as WeatherService from "../api/WeatherService";

vi.mock("../api/WeatherService");

describe("useWeatherDashboard", () => {
  it("fetches weather successfully", async () => {
    WeatherService.getWeatherAndForecast.mockResolvedValueOnce({
      city: "London",
      temperature: 20,
      humidity: 70,
      windSpeed: 5,
      description: "clear sky",
      forecast: [],
      hourly: [],
    });

    const { result } = renderHook(() => useWeatherDashboard(), {
      wrapper: WeatherProvider,
    });

    await act(async () => {
      await result.current.fetchWeather("London");
    });

    expect(result.current.error).toBe("");
    expect(result.current.message).toContain("London");
  });
});

/**
 * @component Forecast
 * 
 * @description
 * Displays the 5-day weather forecast section with daily temperature,
 * condition, and icon for each day.
 * 
 * If no forecast data is available, it renders a friendly fallback message.
 * 
 * This component is purely presentational — all weather data is passed down
 * as props from the parent (e.g., Dashboard or WeatherDisplay) that fetches it
 * through the WeatherContext / WeatherService.
 *
 * @param {Object[]} forecast - Array of forecast objects representing daily weather data.
 * @param {string} forecast[].date - The date of the forecast day.
 * @param {number} forecast[].temp - The average temperature in °C for that day.
 * @param {string} forecast[].description - Short description of the weather condition.
 * @param {string} forecast[].icon - URL of the weather icon image.
 * 
 * @returns {JSX.Element} A responsive grid of forecast cards, or a message when data is missing.
 *
 * @example
 * // Example usage:
 * const forecastData = [
 *   { date: "2025-10-29", temp: 20, description: "Sunny", icon: "https://..." },
 *   { date: "2025-10-30", temp: 18, description: "Partly Cloudy", icon: "https://..." },
 * ];
 * 
 * <Forecast forecast={forecastData} />
 */

import "../styles/DailyForecast.css";
export default function Forecast({ forecast }) {
  if (!forecast || forecast.length === 0) return (      <p className="no-forecast" data-testid="no-forecast">
    No forecast data available
  </p>);

  return (
    <div className="forecast-section" data-testid="forecast-section">
      <h2 className="forecast-title">5-Day Forecast</h2>
      <div className="forecast-grid">
        {forecast.map((day, idx) => (
          <div className="forecast-card" key={idx} data-testid="forecast-card">
            <p className="forecast-date" data-testid="forecast-date">
              {new Date(day.date).toLocaleDateString("en-GB", {
                weekday: "short",
                day: "numeric",
                month: "short",
              })}
            </p>
            <img
              className="forecast-icon"
              src={day.icon}
              alt={day.description}
            />
            <p className="forecast-temp" data-testid="forecast-temp">{Math.round(day.temp)}°C</p>
            <p className="forecast-desc" data-testid="forecast-desc">{day.description}</p>
          </div>
        ))}
      </div>
    </div>
  );
}

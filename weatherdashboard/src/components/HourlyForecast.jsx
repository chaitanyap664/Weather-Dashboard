/**
 * @component HourlyForecast
 *
 * @description
 * Displays the next 12 hours of weather data, including time, temperature, 
 * and condition icons. The component is purely presentational and 
 * renders horizontally scrollable forecast cards.
 *
 * If hourly data is missing or empty, it shows a user-friendly fallback message.
 *
 * This component is typically used within the {@link Dashboard} layout, 
 * receiving its data through the WeatherContext and WeatherService layers.
 *
 * @param {Object[]} hourly - Array of hourly forecast data objects.
 * @param {string} hourly[].time - The forecasted hour (e.g., "13:00").
 * @param {number} hourly[].temp - The temperature in °C at that hour.
 * @param {string} hourly[].icon - The icon URL representing weather conditions.
 *
 * @returns {JSX.Element} A horizontally scrollable list of hourly weather cards 
 * or a message when no data is available.
 *
 * @example
 * const hourlyData = [
 *   { time: "09:00", temp: 18, icon: "https://cdn.weatherapi.com/icons/sunny.png" },
 *   { time: "10:00", temp: 19, icon: "https://cdn.weatherapi.com/icons/cloudy.png" },
 * ];
 * 
 * <HourlyForecast hourly={hourlyData} />
 *
 * @see Dashboard
 * @see WeatherContext
 */

import "../styles/HourlyForecast.css";

export default function HourlyForecast({ hourly }) {
  if (!hourly || hourly.length === 0)  return (
      <p className="no-forecast" data-testid="no-hourly">
        No hourly data available
      </p>
    );

  return (
    <div className="hourly-section" data-testid="hourly-section">
      <h2 className="forecast-title">12 Hours Forecast</h2>
      <div className="hourly-scroll">
        {hourly.map((h, i) => (
          <div className="hourly-card" data-testid="hour-card" key={i}>
            <p className="hour">{h.time}</p>
            <img src={h.icon} alt="hour icon" className="hourly-icon" />
            <p className="hour-temp" data-testid="hour-temp">{Math.round(h.temp)}°C</p>
          </div>
        ))}
      </div>
    </div>
  );
}

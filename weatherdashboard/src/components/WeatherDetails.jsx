/**
 * @component WeatherDetails
 *
 * @description
 * Displays the main weather information for the selected or default city,
 * including temperature, condition, humidity, and wind speed.
 * 
 * The component also adjusts its background style dynamically based on
 * the weather description (e.g., sunny, cloudy, rainy), improving visual feedback.
 *
 * It is a purely presentational component that receives weather data
 * from the parent (typically {@link Dashboard}) via props connected to
 * the WeatherContext and WeatherService layers.
 *
 * @param {Object} weather - The weather data object to display.
 * @param {string} weather.city - The name of the city.
 * @param {number} weather.temp - The current temperature in °C.
 * @param {string} weather.description - Short text describing the weather condition.
 * @param {string} weather.icon - URL of the weather condition icon.
 * @param {number} weather.humidity - Current humidity percentage.
 * @param {number} weather.windSpeed - Current wind speed in km/h.
 *
 * @returns {JSX.Element|null} The formatted weather details section,
 * or null if no weather data is available.
 *
 * @example
 * const sampleWeather = {
 *   city: "London",
 *   temp: 18,
 *   description: "Partly Cloudy",
 *   icon: "https://cdn.weatherapi.com/icons/cloudy.png",
 *   humidity: 70,
 *   windSpeed: 12
 * };
 * 
 * <WeatherDetails weather={sampleWeather} />
 *
 * @see Dashboard
 * @see WeatherContext
 */


import '../styles/WeatherDetails.css';
export default function WeatherDetails({ weather }){
  if (!weather) return null;
  const desc = weather.description?.toLowerCase() || "";
  let bgClass = "weather-clear";
  if (desc.includes("rain")) bgClass = "weather-rainy";
  else if (desc.includes("cloud")) bgClass = "weather-cloudy";
  else if (desc.includes("storm") || desc.includes("thunder")) bgClass = "weather-stormy";
  else if (desc.includes("sunny") || desc.includes("clear")) bgClass = "weather-sunny";
   return (
    <div className={`weather-details ${bgClass}`}>
      <img src={weather.icon} alt={weather.description} className="weather-icon" />
      <div className="city" data-testid="city">{weather.city}</div>
      <div className="temperature" data-testid="temp">{Math.round(weather.temp)}°C</div>
      <div className="description" data-testid="desc">{weather.description}</div>

      <div className="weather-meta">
        <div>
          <strong data-testid="humidity">{weather.humidity}%</strong>
          <span>Humidity</span>
        </div>
        <div>
          <strong data-testid="wind">{weather.windSpeed} km/h</strong>
          <span>Wind</span>
        </div>
      </div>
    </div>
  );
};

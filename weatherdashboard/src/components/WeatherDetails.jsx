import '../styles/WeatherDetails.css';

/**
 * Displays weather data
 */
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

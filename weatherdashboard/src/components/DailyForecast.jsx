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

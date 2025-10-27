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

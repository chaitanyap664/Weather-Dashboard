import { useWeather } from "../context/WeatherContext";
import { useWeatherDashboard } from "../hooks/useWeatherDashboard";
import SearchBar from "../components/SearchBar";
import WeatherDetails from "../components/WeatherDetails";
import Forecast from "../components/DailyForecast";
import Hourly from "../components/HourlyForecast";
import Loader from "./Loader";
import "../styles/Dashboard.css";

export default function Dashboard() {
  const { weather, forecast, hourly } = useWeather();
  const { loading, error, message, fetchWeather, handleSetDefault } = useWeatherDashboard();

  return (
    <div>
      <h1 className="dashboard-title">Weather Dashboard</h1>

      {!loading && !weather && !error && (
        <p className="info-text">Enter a city to view weather information.</p>
      )}

          <SearchBar
        onSearch={fetchWeather}
        onSetDefault={handleSetDefault}
        loading={loading}
      />

     {(message || error) && (
        <div className={`notification ${error ? "error" : "success"}`}>
          {message || error}
        </div>
      )}
      {loading && <Loader />}

      {!loading && !error && weather && (
        <>
          <WeatherDetails weather={weather} />
          <Hourly hourly={hourly} />
          <Forecast forecast={forecast} />
        </>
      )}
    </div>
  );
}

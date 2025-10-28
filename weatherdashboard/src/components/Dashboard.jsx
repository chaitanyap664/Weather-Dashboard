/**
 * @component Dashboard
 *
 * @description
 * The main container component of the Weather Dashboard application.
 * It orchestrates all key UI sections — including the search bar, weather details,
 * hourly forecast, and 5-day forecast — while managing state and data flow
 * through custom hooks and context.
 *
 * This component:
 * - Fetches and displays current, hourly, and daily weather data.
 * - Handles loading, success, and error states gracefully.
 * - Allows users to search for a city or set a default city.
 *
 * It consumes shared weather data via the {@link WeatherContext} and
 * handles business logic through the {@link useWeatherDashboard} custom hook.
 *
 * @returns {JSX.Element} The main weather dashboard UI, including search,
 * notifications, and conditional rendering for weather information.
 *
 * @example
 * // Rendered automatically as the root route or main page of the app
 * <Dashboard />
 *
 * @see useWeatherDashboard
 * @see WeatherContext
 * @see SearchBar
 * @see WeatherDetails
 * @see Hourly
 * @see Forecast
 */

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

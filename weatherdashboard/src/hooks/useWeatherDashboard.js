import { useEffect, useState } from "react";
import { useWeather } from "../context/WeatherContext";
import {
  getWeatherAndForecast,
  setDefaultCity,
  getDefaultCity,
} from "../api/WeatherService";

/**
 * Custom hook that manages all weather-related logic:
 * - Fetching current & forecast data
 * - Handling default city
 * - Displaying success/error messages
 *
 * @returns {Object} Weather dashboard logic and state
 */
export function useWeatherDashboard() {
  const {
    setWeather,
    setForecast,
    setHourly,
  } = useWeather();

  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  // Utility — Clear weather data and reset background
  const clearWeather = () => {
    setWeather(null);
    setForecast([]);
    setHourly([]);
  };

  // Utility — Temporary messages & errors
 const showTempMessage = (text, type = "success") => {
  if (type === "error") setError(text);
  else setMessage(text);

  // Only auto-clear in non-test environments
  if (process.env.NODE_ENV !== "test") {
    setTimeout(() => {
      setError("");
      setMessage("");
    }, 3000);
  }
};

  // Fetch weather for a given city
  const fetchWeather = async (city) => {
    const enteredCity = (city || "").trim();
    if (!enteredCity) {
      console.log("Not entered");
      showTempMessage("Please enter a city name.", "error");
      clearWeather();
      return;
    }

    try {
      setLoading(true);
      setError("");

      const data = await getWeatherAndForecast(enteredCity);
      const newWeather = {
        city: data?.city ?? enteredCity,
        temp: data?.temperature ?? 0,
        humidity: data?.humidity ?? 0,
        windSpeed: data?.windSpeed ?? 0,
        description: data?.description ?? "",
        icon: data?.icon ?? "",
      };

      setWeather(newWeather);
      setForecast(Array.isArray(data?.forecast) ? data.forecast : []);
      setHourly(Array.isArray(data?.hourly) ? data.hourly : []);
      showTempMessage(`Weather data fetched successfully for ${enteredCity}.`);
    } catch (err) {
      showTempMessage(err?.message || "Failed to load weather.", "error");
      clearWeather();
    } finally {
      setLoading(false);
    }
  };

  // Set the default city
  const handleSetDefault = async (city) => {
    const enteredCity = (city || "").trim();
    if (!enteredCity) {
      showTempMessage("Please enter a city before setting default.", "error");
      return;
    }

    try {
      const savedCity = await setDefaultCity(enteredCity);
      showTempMessage(`'${savedCity}' has been set as your default city.`);
    } catch (err) {
      showTempMessage(err?.message || "Failed to set default city.", "error");
    }
  };

  // Load the default city on mount
  useEffect(() => {
    (async () => {
      try {
        const city = await getDefaultCity();
        if (city) {
          await fetchWeather(city);
          showTempMessage(`Showing weather for your default city: ${city}`);
        }
      } catch {
        // No default city set — ignore
      }
    })();
  }, []);

  return {
    loading,
    error,
    message,
    fetchWeather,
    handleSetDefault,
  };
}

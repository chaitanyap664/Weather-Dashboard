// src/context/WeatherContext.js
import { createContext, useContext, useState } from "react";

// Create the context
export const WeatherContext = createContext(null);

// Hook to use context easily
export const useWeather = () => useContext(WeatherContext);

// Provider component wrapping the app
export function WeatherProvider({ children }) {
  const [weather, setWeather] = useState(null);
  const [forecast, setForecast] = useState([]);
  const [hourly, setHourly] = useState([]);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  return (
    <WeatherContext.Provider
      value={{
        weather,
        setWeather,
        forecast,
        setForecast,
        hourly,
        setHourly,
        error,
        setError,
        loading,
        setLoading,
      }}
    >
      {children}
    </WeatherContext.Provider>
  );
}

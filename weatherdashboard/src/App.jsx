// src/App.jsx
import React from "react";
import Dashboard from "./components/Dashboard";
import { WeatherProvider, useWeather } from "./context/WeatherContext";
import "./styles/App.css";

// Separate the background logic into a wrapper
function AppWrapper() {
  const { background } = useWeather();
  return (
    <div className={`app-background ${background}`}>
      <div className="app-overlay">
        <Dashboard />
      </div>
    </div>
  );
}

export default function App() {
  return (
    <WeatherProvider>
      <AppWrapper />
    </WeatherProvider>
  );
}

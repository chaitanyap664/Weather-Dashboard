import React, { useState } from "react";
import "../styles/search.css";

/**
 * SearchBar Component
 * Handles user input for searching weather and setting default location.
 *
 * @param {Function} onSearch - Triggered when user clicks "Search"
 * @param {Function} onSetDefault - Triggered when user clicks "Set Default"
 */
export default function SearchBar({ onSearch, onSetDefault }) {
  const [city, setCity] = useState("");

  const handleSubmit = (e) => {
    e.preventDefault();
      onSearch?.(city.trim());
  };

  const handleSetDefault = (e) => {
    e.preventDefault();
      onSetDefault?.(city.trim());
  };

  return (
    <form className="search-bar" onSubmit={handleSubmit}>
      <input
        type="text"
        placeholder="Enter city name"
        aria-label="Enter city name"
        value={city}
        onChange={(e) => setCity(e.target.value)}
        autoFocus
      />
      <button
        type="submit"
        className="search-btn"
      >
        Search
      </button>
      <button
        type="button"
        className="search-btn secondary"
        onClick={handleSetDefault}
      >
        Set Default
      </button>
    </form>
  );
}

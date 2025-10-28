/**
 * @component Loader
 *
 * @description
 * Displays a loading spinner and message while weather data 
 * is being fetched from the API.  
 * 
 * This component provides visual feedback to users during 
 * asynchronous operations (e.g., API calls triggered by search 
 * or default city load) and enhances perceived responsiveness.
 *
 * It is rendered conditionally within the {@link Dashboard} 
 * component whenever the `loading` state is true.
 *
 * @returns {JSX.Element} A centered spinner animation with 
 * a descriptive loading message.
 *
 * @example
 * // Example usage within Dashboard:
 * {loading && <Loader />}
 *
 * @see Dashboard
 */

import "../styles/Loader.css";

export default function Loader() {
  return (
    <div className="loader-wrapper">
      <div className="spinner"></div>
      <p className="loading-text" data-testid="loading">Fetching latest weather...</p>
    </div>
  );
}
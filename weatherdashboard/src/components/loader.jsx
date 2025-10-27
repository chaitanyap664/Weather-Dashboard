import "../styles/Loader.css";

export default function Loader() {
  return (
    <div className="loader-wrapper">
      <div className="spinner"></div>
      <p className="loading-text" data-testid="loading">Fetching latest weather...</p>
    </div>
  );
}
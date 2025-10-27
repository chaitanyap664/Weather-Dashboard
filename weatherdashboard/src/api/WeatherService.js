/**
 * Fetches weather details (current + 6-day forecast) for a given city.
 * @param {string} city - The city to fetch data for.
 * @returns {Promise<Object>} Weather response from backend API.
 */

const BASE_URL = import.meta.env.VITE_API_BASE_URL;

export async function fetchWeatherAsync(city) {
  const response = await fetch(
    `${BASE_URL}?city=${encodeURIComponent(city)}&days=6`
  );
  // Read once and parse safely
  let data;
  try {
    data = await response.json();
  } catch {
    data = null;
  }

  if (!response.ok) {
    // if already read, we can’t call res.text() again — use parsed data or fallback message
    const message =
      (data && data.message) ||
      `Failed to fetch weather for ${city} (${response.status})`;
    throw new Error(message);
  }

  // Expected shape (flat from your .NET API)
  // { city, temp, humidity, windSpeed, description, icon, forecast:[], hourly:[] }
  return data;
}


/* * Get the currently saved default city from the backend.
 */
export async function fetchDefaultCityAsync() {
  const response = await fetch(`${BASE_URL}/default`);
  if (response.status === 404) return null;
  if (!response.ok) throw new Error("Failed to get default city");
  const data = await response.json();
  return data.city;
}

export async function updateDefaultCityAsync(city) {
  const response = await fetch(`${BASE_URL}/default`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ city }),
  });

  const data = await response.json().catch(() => null);

  if (!response.ok) {
    const msg = (data && data.message) || "Failed to set default city.";
    throw new Error(msg);
  }

  // Extract only city name safely
  const message = data?.message || "";
  const cityMatch = message.match(/'([^']+)'/); // capture what's inside quotes
  const cleanCity = cityMatch ? cityMatch[1] : city;
  return cleanCity; // return only the clean city name
}
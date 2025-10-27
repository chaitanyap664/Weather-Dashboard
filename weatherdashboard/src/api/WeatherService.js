const BASE_URL = import.meta.env.VITE_API_BASE_URL;

export async function getWeatherAndForecast(city) {
  const res = await fetch(
    `${BASE_URL}?city=${encodeURIComponent(city)}&days=6`
  );
  // Read once and parse safely
  let data;
  try {
    data = await res.json();
  } catch {
    data = null;
  }

  if (!res.ok) {
    // if already read, we can’t call res.text() again — use parsed data or fallback message
    const message =
      (data && data.message) ||
      `Failed to fetch weather for ${city} (${res.status})`;
    throw new Error(message);
  }

  // Expected shape (flat from your .NET API)
  // { city, temp, humidity, windSpeed, description, icon, forecast:[], hourly:[] }
  return data;
}


/* * Get the currently saved default city from the backend.
 */
export async function getDefaultCity() {
  const res = await fetch(`${BASE_URL}/default`);
  if (res.status === 404) return null;
  if (!res.ok) throw new Error("Failed to get default city");
  const data = await res.json();
  return data.city;
}

export async function setDefaultCity(city) {
  const res = await fetch(`${BASE_URL}/default`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ city }),
  });

  const data = await res.json().catch(() => null);

  if (!res.ok) {
    const msg = (data && data.message) || "Failed to set default city.";
    throw new Error(msg);
  }

  // Extract only city name safely
  const message = data?.message || "";
  const cityMatch = message.match(/'([^']+)'/); // capture what's inside quotes
  const cleanCity = cityMatch ? cityMatch[1] : city;

  return cleanCity; // return only the clean city name
}
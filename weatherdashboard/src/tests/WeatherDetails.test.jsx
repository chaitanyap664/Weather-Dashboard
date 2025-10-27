import { render, screen } from "@testing-library/react";
import WeatherDetails from "../components/WeatherDetails";

test("renders main weather info", () => {
  const weather = {
    city: "New York",
    temp: 14,
    description: "clear sky",
    humidity: 54,
    windSpeed: 7.2,
    icon: "01d",
  };

  render(<WeatherDetails weather={weather} />);
  expect(screen.getByTestId("temp")).toHaveTextContent("14°C");
  expect(screen.getByTestId("city")).toHaveTextContent("New York");
  expect(screen.getByTestId("desc")).toHaveTextContent("clear sky");
  expect(screen.getByTestId("humidity")).toHaveTextContent("54%");
  expect(screen.getByTestId("wind")).toHaveTextContent("7.2 km/h");
  expect(screen.getByAltText(/clear sky/i)).toBeInTheDocument();
});

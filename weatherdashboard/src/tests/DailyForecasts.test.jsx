import { render, screen } from "@testing-library/react";
import Forecast from "../components/DailyForecast";

describe("Forecast Component", () => {
  const mockForecast = [
    {
      date: "2025-10-23",
      temp: 16,
      description: "Sunny",
      icon: "https://cdn.weatherapi.com/weather/64x64/day/113.png",
    },
    {
      date: "2025-10-24",
      temp: 14,
      description: "Cloudy",
      icon: "https://cdn.weatherapi.com/weather/64x64/day/116.png",
    },
  ];

  test("renders forecast cards when data is available", () => {
    render(<Forecast forecast={mockForecast} />);
    const cards = screen.getAllByTestId("forecast-card");
    expect(cards.length).toBe(2);
    expect(screen.getByText(/23\s*oct/i)).toBeInTheDocument();
    expect(screen.getByText("Sunny")).toBeInTheDocument();
  });

  test("renders message when forecast data is empty", () => {
    render(<Forecast forecast={[]} />);
    expect(screen.getByTestId("no-forecast")).toBeInTheDocument();
  });
});

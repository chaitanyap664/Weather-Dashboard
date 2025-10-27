import { render, screen } from "@testing-library/react";
import Hourly from "../components/HourlyForecast";

describe("Hourly Component", () => {
  const mockHourly = [
    { time: "09:00", temp: 15, icon: "https://cdn.weatherapi.com/weather/64x64/day/113.png" },
    { time: "10:00", temp: 16, icon: "https://cdn.weatherapi.com/weather/64x64/day/116.png" },
  ];

  test("renders hourly cards when data is available", () => {
    render(<Hourly hourly={mockHourly} />);
    const cards = screen.getAllByTestId("hour-card");
    expect(cards.length).toBe(2);
    expect(screen.getByText("09:00")).toBeInTheDocument();
  });

  test("renders message when hourly data is empty", () => {
    render(<Hourly hourly={[]} />);
    expect(screen.getByTestId("no-hourly")).toBeInTheDocument();
  });
});

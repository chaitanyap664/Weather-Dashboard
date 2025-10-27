import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { vi } from "vitest";
import Dashboard from "../components/Dashboard";
import { WeatherProvider } from "../context/WeatherContext";
import * as WeatherService from "../api/WeatherService";

// Mock API calls

//Mock the WeatherService module
vi.mock("../api/WeatherService", () => ({
  fetchDefaultCityAsync: vi.fn(),
  updateDefaultCityAsync: vi.fn(),
   fetchWeatherAsync: vi.fn(),
}));

const renderWithContext = (ui) => {
  return render(<WeatherProvider>{ui}</WeatherProvider>);
};

describe("Dashboard", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders search bar and title", () => {
    renderWithContext(<Dashboard />);
    expect(screen.getByText(/weather dashboard/i)).toBeInTheDocument();
    expect(screen.getByPlaceholderText(/enter city/i)).toBeInTheDocument();
  });

  it("loads weather after searching a city", async () => {
    WeatherService.fetchWeatherAsync.mockResolvedValue({
      city: "London",
      temperature: 20,
      humidity: 60,
      windSpeed: 12,
      description: "Sunny",
      icon: "icon.png",
      forecast: [],
      hourly: [],
    });

    renderWithContext(<Dashboard />);
  
    const input = screen.getByPlaceholderText(/enter city name/i);
    const searchBtn = screen.getByRole("button", { name: /search/i });

    fireEvent.change(input, { target: { value: "London" } });
    fireEvent.click(searchBtn);

    // Wait for city to render
    await waitFor(() =>
      expect(screen.getByTestId("city")).toHaveTextContent("London")
    );

    //Assert other key UI elements without using ambiguous `findByText`
    expect(screen.getByTestId("temp")).toHaveTextContent("20");
    expect(screen.getByTestId("humidity")).toHaveTextContent("60");
    expect(screen.getByTestId("wind")).toHaveTextContent("12");

    //Optional: check that success message appeared
    expect(
      screen.getByText(/weather data fetched successfully for london/i)
    ).toBeInTheDocument();
  });

  it("sets default city and shows success message", async () => {
    WeatherService.updateDefaultCityAsync.mockResolvedValue("Paris");
    WeatherService.fetchDefaultCityAsync.mockResolvedValue({
      city: "Paris",
      temperature: 24,
      humidity: 55,
      windSpeed: 10,
      description: "Clear",
      icon: "icon.png",
      forecast: [],
      hourly: [],
    });

    renderWithContext(<Dashboard />);

    const input = screen.getByPlaceholderText(/enter city/i);
    fireEvent.change(input, { target: { value: "Paris" } });
    fireEvent.click(screen.getByRole("button", { name: /set default/i }));

    await waitFor(() =>
      expect(WeatherService.updateDefaultCityAsync).toHaveBeenCalledWith("Paris")
    );

    expect(await screen.findByText(/has been set as your default city/i)).toBeInTheDocument();
    expect(await screen.findByText(/paris/i)).toBeInTheDocument();
  });
  it("shows error message when trying to set default city without entering one", async () => {
  renderWithContext(<Dashboard />);

  // Click 'Set Default' without typing anything
  const setDefaultButton = screen.getByRole("button", { name: /set default/i });
  fireEvent.click(setDefaultButton);

  // Expect an error message to appear
  await waitFor(() => {
    expect(
      screen.getByText(/please enter a city before setting default/i)
    ).toBeInTheDocument();
  });

  // Verify WeatherService.setDefaultCity is NOT called
  expect(WeatherService.updateDefaultCityAsync).not.toHaveBeenCalled();
});
});
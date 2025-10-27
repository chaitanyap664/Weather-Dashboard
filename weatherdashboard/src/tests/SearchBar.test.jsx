import { render, screen, fireEvent } from "@testing-library/react";
import SearchBar from "../components/SearchBar";

describe("SearchBar", () => {
  it("renders input and both buttons", () => {
    render(<SearchBar onSearch={() => {}} onSetDefault={() => {}} />);

    expect(screen.getByPlaceholderText(/enter city/i)).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /search/i })).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /set default/i })
    ).toBeInTheDocument();
  });

  it("updates input value when typing", () => {
    render(<SearchBar onSearch={() => {}} onSetDefault={() => {}} />);
    const input = screen.getByPlaceholderText(/enter city/i);
    fireEvent.change(input, { target: { value: "London" } });
    expect(input.value).toBe("London");
  });

  it("calls onSearch when Search clicked", () => {
    const onSearch = vi.fn();
    render(<SearchBar onSearch={onSearch} onSetDefault={() => {}} />);
    const input = screen.getByPlaceholderText(/enter city/i);
    fireEvent.change(input, { target: { value: "Paris" } });
    fireEvent.click(screen.getByRole("button", { name: /search/i }));
    expect(onSearch).toHaveBeenCalledWith("Paris");
  });

  it("calls onSetDefault when Set Default clicked", () => {
    const onSetDefault = vi.fn();
    render(<SearchBar onSearch={() => {}} onSetDefault={onSetDefault} />);
    const input = screen.getByPlaceholderText(/enter city/i);
    fireEvent.change(input, { target: { value: "Rome" } });
    fireEvent.click(screen.getByRole("button", { name: /set default/i }));
    expect(onSetDefault).toHaveBeenCalledWith("Rome");
  });
it("keeps buttons enabled even when input is empty", () => {
  render(<SearchBar onSearch={vi.fn()} onSetDefault={vi.fn()} />);
  expect(screen.getByRole("button", { name: /search/i })).toBeEnabled();
  expect(screen.getByRole("button", { name: /set default/i })).toBeEnabled();
});
});
import { act, fireEvent, render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { afterEach, beforeEach, describe, expect, test, vi } from "vitest";
import SearchBar from "./SearchBar";
import SearchBarMobile from "./SearchBarMobile";

const { toastInfo, toastError } = vi.hoisted(() => ({
  toastInfo: vi.fn(),
  toastError: vi.fn(),
}));

vi.mock("react-toastify", () => ({
  toast: {
    info: toastInfo,
    error: toastError,
  },
}));

vi.mock("./LupaWidget", () => ({
  default: ({ onClick }) => (
    <button type="button" data-testid="search-trigger" onClick={onClick}>
      search
    </button>
  ),
}));

vi.mock("./SearchResults", () => ({
  default: () => null,
}));

vi.mock("./LoadingSVG", () => ({
  default: () => null,
}));

const successfulEmptySearch = () =>
  Promise.resolve({
    ok: true,
    json: () =>
      Promise.resolve({
        isSuccess: true,
        data: { $values: [] },
      }),
  });

beforeEach(() => {
  vi.useFakeTimers();
  globalThis.fetch = vi.fn(successfulEmptySearch);
});

afterEach(() => {
  vi.clearAllTimers();
  vi.useRealTimers();
  vi.restoreAllMocks();
});

async function advanceDebounce() {
  await act(async () => {
    vi.advanceTimersByTime(500);
    await Promise.resolve();
    await Promise.resolve();
  });
}

describe("search debounce contracts", () => {
  test("SearchBar performs one mobile search after the maintained 500ms debounce", async () => {
    render(
      <MemoryRouter>
        <SearchBar isVisible onClose={vi.fn()} isMobile />
      </MemoryRouter>
    );

    fireEvent.change(screen.getByPlaceholderText("Buscar producto"), {
      target: { value: "avena integral" },
    });

    act(() => {
      vi.advanceTimersByTime(499);
    });
    expect(globalThis.fetch).not.toHaveBeenCalled();

    await advanceDebounce();

    expect(globalThis.fetch).toHaveBeenCalledTimes(1);
    expect(globalThis.fetch).toHaveBeenCalledWith(
      expect.stringContaining("avena%20integral"),
      expect.objectContaining({ method: "GET" })
    );
  });

  test("SearchBar does not debounce-search when rendered as desktop", async () => {
    render(
      <MemoryRouter>
        <SearchBar isVisible onClose={vi.fn()} isMobile={false} />
      </MemoryRouter>
    );

    fireEvent.change(screen.getByPlaceholderText("Buscar producto"), {
      target: { value: "granola" },
    });

    await advanceDebounce();

    expect(globalThis.fetch).not.toHaveBeenCalled();
  });

  test("SearchBarMobile performs one search after the maintained 500ms debounce", async () => {
    render(<SearchBarMobile />);

    fireEvent.change(screen.getByPlaceholderText("Buscar producto"), {
      target: { value: "barrita" },
    });

    await advanceDebounce();

    expect(globalThis.fetch).toHaveBeenCalledTimes(1);
    expect(globalThis.fetch).toHaveBeenCalledWith(
      expect.stringContaining("barrita"),
      expect.objectContaining({ method: "GET" })
    );
  });
});

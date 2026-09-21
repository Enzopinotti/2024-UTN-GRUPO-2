import { cleanup, render, screen } from "@testing-library/react";
import { useOutletContext } from "react-router-dom";
import { afterEach, beforeEach, describe, expect, test, vi } from "vitest";
import Orders from "./Orders";

vi.mock("react-router-dom", () => ({
  useOutletContext: vi.fn(),
}));

vi.mock("../../components/users/Orders/UserOrderListContainer", () => ({
  default: ({ orders }) => (
    <ul data-testid="orders">
      {orders.map((order) => (
        <li key={order.id}>{order.orderNumber}</li>
      ))}
    </ul>
  ),
}));

let contextValue;
let fetchMock;

beforeEach(() => {
  cleanup();
  vi.clearAllMocks();
  contextValue = { user: { id: 1 } };
  vi.mocked(useOutletContext).mockImplementation(() => contextValue);
  fetchMock = vi.fn();
  vi.stubGlobal("fetch", fetchMock);
});

afterEach(() => {
  vi.unstubAllGlobals();
});

describe("Profile Orders local-data authority", () => {
  test("filters demo orders by the outlet user and never calls a backend placeholder", () => {
    const { rerender } = render(<Orders />);

    expect(screen.getByText("ORD-20231001-001")).toBeInTheDocument();
    expect(screen.getByText("ORD-20231003-002")).toBeInTheDocument();
    expect(screen.getByText("ORD-20231008-004")).toBeInTheDocument();
    expect(screen.queryByText("ORD-20231005-003")).not.toBeInTheDocument();
    expect(fetchMock).not.toHaveBeenCalled();

    contextValue = { user: { id: 2 } };
    rerender(<Orders />);

    expect(screen.getByText("ORD-20231005-003")).toBeInTheDocument();
    expect(screen.queryByText("ORD-20231001-001")).not.toBeInTheDocument();
    expect(screen.queryByText("ORD-20231003-002")).not.toBeInTheDocument();
    expect(screen.queryByText("ORD-20231008-004")).not.toBeInTheDocument();
    expect(fetchMock).not.toHaveBeenCalled();
  });
});

import { afterEach, beforeEach, describe, expect, test, vi } from "vitest";
import { authenticatedFetch } from "./authenticatedFetch";

describe("authenticatedFetch", () => {
  beforeEach(() => {
    localStorage.clear();
    vi.stubGlobal("fetch", vi.fn().mockResolvedValue({ ok: true }));
  });

  afterEach(() => {
    vi.unstubAllGlobals();
    localStorage.clear();
  });

  test("fails closed before making a request when the access token is absent", async () => {
    await expect(
      authenticatedFetch("https://api.example.test/protected")
    ).rejects.toThrow("Se requiere una sesión autenticada");

    expect(fetch).not.toHaveBeenCalled();
  });

  test("adds the persisted Bearer token while preserving caller headers", async () => {
    localStorage.setItem("accessToken", "jwt-access-token");

    await authenticatedFetch("https://api.example.test/protected", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "X-Request-Source": "admin-products",
      },
      body: "{}",
    });

    expect(fetch).toHaveBeenCalledTimes(1);

    const [url, options] = vi.mocked(fetch).mock.calls[0];
    expect(url).toBe("https://api.example.test/protected");
    expect(options.method).toBe("POST");
    expect(options.body).toBe("{}");
    expect(options.headers).toBeInstanceOf(Headers);
    expect(options.headers.get("Authorization")).toBe("Bearer jwt-access-token");
    expect(options.headers.get("Content-Type")).toBe("application/json");
    expect(options.headers.get("X-Request-Source")).toBe("admin-products");
  });
});

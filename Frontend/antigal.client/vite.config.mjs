import { defineConfig, transformWithOxc } from "vite";
import react from "@vitejs/plugin-react";

const jsxInLegacyJs = () => ({
  name: "antigal-jsx-in-legacy-js",
  enforce: "pre",
  async transform(code, id) {
    if (!/\/src\/.*\.js$/.test(id)) {
      return null;
    }

    const transformed = await transformWithOxc(code, id, {
      lang: "jsx",
      jsx: {
        runtime: "automatic",
      },
    });

    return {
      code: transformed.code,
      map: transformed.map ?? null,
    };
  },
});

export default defineConfig({
  plugins: [
    jsxInLegacyJs(),
    react({
      include: /\.[jt]sx?$/,
    }),
  ],
  optimizeDeps: {
    esbuildOptions: {
      loader: {
        ".js": "jsx",
      },
    },
  },
  server: {
    port: 3000,
    strictPort: true,
  },
  preview: {
    port: 3000,
    strictPort: true,
  },
  build: {
    outDir: "build",
  },
  test: {
    environment: "jsdom",
    environmentOptions: {
      jsdom: {
        url: "http://localhost:3000/",
      },
    },
    globals: true,
    setupFiles: "./src/setupTests.js",
    css: true,
  },
});

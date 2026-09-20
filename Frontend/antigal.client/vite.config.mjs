import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
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
    rolldownOptions: {
      output: {
        codeSplitting: {
          groups: [
            {
              name: "react-runtime",
              test: /node_modules[\\/](?:react|react-dom|scheduler)[\\/]/,
              priority: 20,
            },
          ],
        },
      },
    },
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

import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import { TanStackRouterVite } from "@tanstack/router-plugin/vite";

const ReactCompilerConfig = {
  target: "19",
  sources: (filename: string) => {
    return filename.includes("src");
  },
};

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react({
      babel: {
        plugins: [["babel-plugin-react-compiler", ReactCompilerConfig]],
      },
    }),
    TanStackRouterVite(),
  ],
});

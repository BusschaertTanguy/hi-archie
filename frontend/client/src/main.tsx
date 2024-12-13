import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { createRouter, RouterProvider } from "@tanstack/react-router";
import { routeTree } from "./routeTree.gen.ts";
import { Auth0Provider } from "@auth0/auth0-react";
import { authConfig } from "./configs/auth-config.ts";
import UserProvider from "./providers/user-provider.tsx";
import AxiosProvider from "./providers/axios-provider.tsx";

const root = document.getElementById("root");

if (!root) {
  throw new Error("Root not found");
}

const queryClient = new QueryClient();
const router = createRouter({ routeTree });

declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router;
  }
}

createRoot(root).render(
  <StrictMode>
    <Auth0Provider {...authConfig}>
      <AxiosProvider>
        <QueryClientProvider client={queryClient}>
          <UserProvider>
            <RouterProvider router={router} />
          </UserProvider>
          <ReactQueryDevtools initialIsOpen={false} />
        </QueryClientProvider>
      </AxiosProvider>
    </Auth0Provider>
  </StrictMode>,
);

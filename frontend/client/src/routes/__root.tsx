import { createRootRoute, Outlet } from "@tanstack/react-router";
import { lazy, Suspense } from "react";
import Header from "../components/header.tsx";
import SideNav from "../components/side-nav.tsx";
import { z } from "zod";

const TanStackRouterDevtools = import.meta.env.PROD
  ? () => null
  : lazy(() =>
      import("@tanstack/router-devtools").then((res) => ({
        default: res.TanStackRouterDevtools,
      })),
    );

const Root = () => (
  <>
    <div className="flex h-full flex-col divide-y">
      <Header />
      <div className="flex flex-1 divide-x">
        <SideNav />
        <main className="flex flex-1 flex-col items-center">
          <div className="-ml-32 w-1/2 p-6">
            <Outlet />
          </div>
        </main>
      </div>
    </div>
    <Suspense>
      <TanStackRouterDevtools />
    </Suspense>
  </>
);

const rootSearchSchema = z.object({
  search: z.string().optional(),
});

export const Route = createRootRoute({
  component: Root,
  validateSearch: (search) => rootSearchSchema.parse(search),
});

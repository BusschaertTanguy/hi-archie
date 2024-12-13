import { createRootRoute, Outlet } from "@tanstack/react-router";
import { lazy, Suspense } from "react";
import Header from "../components/header.tsx";
import SideNav from "../components/side-nav.tsx";

const TanStackRouterDevtools = import.meta.env.PROD
  ? () => null
  : lazy(() =>
      import("@tanstack/router-devtools").then((res) => ({
        default: res.TanStackRouterDevtools,
      })),
    );

const Root = () => {
  return (
    <>
      <div className="flex h-full flex-col divide-y">
        <Header />
        <div className="flex flex-1 divide-x">
          <SideNav />
          <main className="flex-1">
            <Outlet />
          </main>
        </div>
      </div>
      <Suspense>
        <TanStackRouterDevtools />
      </Suspense>
    </>
  );
};

export const Route = createRootRoute({
  component: Root,
});

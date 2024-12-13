import { createLazyFileRoute, Navigate } from "@tanstack/react-router";

export const Route = createLazyFileRoute("/")({
  component: Index,
});

function Index() {
  return <Navigate to="/communities" search={{ pageIndex: 0, pageSize: 25 }} />;
}

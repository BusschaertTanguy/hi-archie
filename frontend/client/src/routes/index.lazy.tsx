import { createLazyFileRoute, Navigate } from "@tanstack/react-router";

const Index = () => (
  <Navigate to="/communities" search={{ pageIndex: 0, pageSize: 25 }} />
);

export const Route = createLazyFileRoute("/")({
  component: Index,
});

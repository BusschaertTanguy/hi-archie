import { createFileRoute } from "@tanstack/react-router";
import { z } from "zod";

const productSearchSchema = z.object({
  pageIndex: z.number().min(0).catch(0),
  pageSize: z.literal(25).catch(25),
});

export const Route = createFileRoute("/communities/$communityId/")({
  validateSearch: (search) => productSearchSchema.parse(search),
});

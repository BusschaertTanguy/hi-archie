import { createLazyFileRoute } from "@tanstack/react-router";
import { useGetApiV1CommunitiesId } from "../../../api/types";
import Button from "../../../components/button.tsx";
import { PlusIcon } from "@heroicons/react/24/solid";
import useUser from "../../../hooks/use-user.ts";

export const Route = createLazyFileRoute("/communities/$communityId/")({
  component: Community,
});

function Community() {
  const { userId } = useUser();
  const { communityId } = Route.useParams();
  const communityQuery = useGetApiV1CommunitiesId(communityId);

  return (
    <div className="flex flex-col gap-5">
      <div className="flex items-center justify-between">
        <div className="text-2xl">{communityQuery.data?.name}</div>
        {userId && (
          <Button
            color="black"
            variant="filled"
            className="flex items-center justify-center gap-0.5"
          >
            <PlusIcon className="size-6" />
            POST
          </Button>
        )}
      </div>
    </div>
  );
}

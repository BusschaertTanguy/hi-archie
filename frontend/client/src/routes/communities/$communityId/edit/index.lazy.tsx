import { createLazyFileRoute } from "@tanstack/react-router";
import {
  useGetApiV1CommunitiesId,
  usePutApiV1Communities,
} from "../../../../api/types";
import CommunityForm from "../../-components/community-form.tsx";
import { useCallback } from "react";

const EditCommunity = () => {
  const { communityId } = Route.useParams();
  const navigate = Route.useNavigate();

  const navigateBack = useCallback(
    () =>
      navigate({
        to: "/communities/$communityId",
        params: { communityId },
        search: { pageIndex: 0, pageSize: 25 },
      }),
    [communityId, navigate],
  );

  const communityQuery = useGetApiV1CommunitiesId(communityId);

  const editMutation = usePutApiV1Communities({
    mutation: { onSuccess: navigateBack },
  });

  return (
    <div className="flex flex-col items-center">
      <CommunityForm
        community={communityQuery.data}
        onCancel={navigateBack}
        onSubmit={(data) =>
          editMutation.mutateAsync({
            data: {
              id: communityId,
              ...data,
            },
          })
        }
      />
    </div>
  );
};

export const Route = createLazyFileRoute("/communities/$communityId/edit/")({
  component: EditCommunity,
});

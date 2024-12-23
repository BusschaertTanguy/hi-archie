import { createLazyFileRoute } from "@tanstack/react-router";
import CommunityForm from "../-components/community-form.tsx";
import { usePostApiV1Communities } from "../../../api/types";
import { useCallback } from "react";

const AddCommunity = () => {
  const navigate = Route.useNavigate();

  const navigateBack = useCallback(
    () =>
      navigate({
        to: "/communities",
        search: {
          pageIndex: 0,
          pageSize: 25,
        },
      }),
    [navigate],
  );

  const addCommunityMutation = usePostApiV1Communities({
    mutation: { onSuccess: navigateBack },
  });

  return (
    <div className="flex flex-col items-center">
      <CommunityForm
        onCancel={navigateBack}
        onSubmit={(data) =>
          addCommunityMutation.mutateAsync({
            data: {
              id: crypto.randomUUID(),
              ...data,
            },
          })
        }
      />
    </div>
  );
};

export const Route = createLazyFileRoute("/communities/add/")({
  component: AddCommunity,
});

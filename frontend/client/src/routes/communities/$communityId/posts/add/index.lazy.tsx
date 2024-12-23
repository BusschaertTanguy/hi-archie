import { createLazyFileRoute, Navigate } from "@tanstack/react-router";
import { usePostApiV1Posts } from "../../../../../api/types";
import useUser from "../../../../../hooks/use-user.ts";
import { useCallback } from "react";
import PostForm from "../-components/post-form.tsx";

const AddPost = () => {
  const { joinedCommunities } = useUser();
  const addPostMutation = usePostApiV1Posts();
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

  if (!joinedCommunities?.includes(communityId)) {
    return (
      <Navigate
        to="/communities/$communityId"
        params={{ communityId }}
        search={{ pageIndex: 0, pageSize: 25 }}
      />
    );
  }

  return (
    <PostForm
      onSubmit={async (data) => {
        await addPostMutation.mutateAsync({
          data: {
            id: crypto.randomUUID(),
            communityId,
            ...data,
          },
        });

        await navigateBack();
      }}
      onCancel={navigateBack}
    />
  );
};

export const Route = createLazyFileRoute(
  "/communities/$communityId/posts/add/",
)({
  component: AddPost,
});

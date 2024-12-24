import { createLazyFileRoute } from "@tanstack/react-router";
import {
  useGetApiV1PostsId,
  usePutApiV1Posts,
} from "../../../../../../api/types";
import { useCallback } from "react";
import PostForm from "../../../-components/post-form.tsx";

const EditPost = () => {
  const { communityId, postId } = Route.useParams();

  const navigate = Route.useNavigate();

  const navigateBack = useCallback(
    () =>
      navigate({
        to: "/communities/$communityId/posts/$postId",
        params: { communityId, postId },
      }),
    [communityId, postId, navigate],
  );

  const postQuery = useGetApiV1PostsId(postId);

  const editMutation = usePutApiV1Posts({
    mutation: { onSuccess: navigateBack },
  });

  return (
    <PostForm
      post={postQuery.data}
      onSubmit={async (data) => {
        await editMutation.mutateAsync({
          data: {
            id: postId,
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
  "/communities/$communityId/posts/$postId/edit/",
)({
  component: EditPost,
});

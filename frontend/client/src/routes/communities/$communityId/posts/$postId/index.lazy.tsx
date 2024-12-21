import { createLazyFileRoute } from "@tanstack/react-router";
import { useGetApiV1PostsId } from "../../../../../api/types";

export const Route = createLazyFileRoute(
  "/communities/$communityId/posts/$postId/",
)({
  component: Post,
});

function Post() {
  const { postId } = Route.useParams();
  const postQuery = useGetApiV1PostsId(postId);

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col gap-2">
        <div className="text-2xl">{postQuery.data?.title}</div>
        <div className="text-xs">
          {new Date(postQuery.data?.publishDate ?? "").toDateString()}
        </div>
      </div>
      <div className="rounded border border-black p-3">
        {postQuery.data?.content}
      </div>
    </div>
  );
}

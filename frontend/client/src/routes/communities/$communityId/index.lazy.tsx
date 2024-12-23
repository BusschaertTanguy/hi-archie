import { createLazyFileRoute, Link } from "@tanstack/react-router";
import {
  useGetApiV1CommunitiesId,
  useGetApiV1Posts,
  usePostApiV1CommunitiesJoin,
  usePostApiV1CommunitiesLeave,
} from "../../../api/types";
import Button from "../../../components/button.tsx";
import useUser from "../../../hooks/use-user.ts";

const Community = () => {
  const { userId, joinedCommunities, refresh } = useUser();

  const navigate = Route.useNavigate();
  const { communityId } = Route.useParams();
  const { pageIndex, pageSize, search } = Route.useSearch();

  const communityQuery = useGetApiV1CommunitiesId(communityId);
  const postsQuery = useGetApiV1Posts({
    pageIndex,
    pageSize,
    communityId,
    title: search,
  });

  const leaveMutation = usePostApiV1CommunitiesLeave({
    mutation: {
      onSuccess: () => {
        refresh();
      },
    },
  });

  const joinMutation = usePostApiV1CommunitiesJoin({
    mutation: {
      onSuccess: () => {
        refresh();
      },
    },
  });

  return (
    <div className="flex flex-col gap-5">
      <div className="flex items-center justify-between">
        <div className="text-2xl">{communityQuery.data?.name}</div>
        {userId && joinedCommunities?.includes(communityId) && (
          <div className="flex gap-2">
            <Button
              color="black"
              variant="filled"
              className="flex items-center justify-center gap-0.5"
              onClick={() =>
                navigate({
                  to: "/communities/$communityId/posts/add",
                  params: { communityId },
                })
              }
            >
              POST
            </Button>
            <Button
              color="black"
              variant="outlined"
              className="flex items-center justify-center gap-0.5"
              onClick={() =>
                leaveMutation.mutateAsync({
                  data: {
                    communityId,
                  },
                })
              }
            >
              LEAVE
            </Button>
          </div>
        )}
        {userId && !joinedCommunities?.includes(communityId) && (
          <Button
            color="black"
            variant="filled"
            className="flex items-center justify-center gap-0.5"
            onClick={() =>
              joinMutation.mutateAsync({
                data: {
                  communityId,
                },
              })
            }
          >
            JOIN
          </Button>
        )}
      </div>
      <div className="flex flex-col gap-4">
        {postsQuery.data?.posts.map((post) => (
          <div
            key={post.id}
            className="rounded p-4 outline outline-1 outline-black"
            onClick={() =>
              navigate({
                to: "/communities/$communityId/posts/$postId",
                params: {
                  communityId,
                  postId: post.id,
                },
              })
            }
          >
            {post.title}
          </div>
        ))}
      </div>
      <div className="flex items-center justify-end gap-4">
        <Link
          className="rounded px-3 py-1 text-black outline outline-1 outline-black"
          from="/communities/$communityId"
          to="."
          search={(prev) => ({ ...prev, pageIndex: prev.pageIndex - 1 })}
          disabled={pageIndex === 0}
        >
          PREVIOUS
        </Link>
        <Link
          className="rounded px-3 py-1 text-black outline outline-1 outline-black"
          from="/communities/$communityId"
          to="."
          search={(prev) => ({ ...prev, pageIndex: prev.pageIndex + 1 })}
          disabled={(pageIndex + 1) * pageSize >= (postsQuery.data?.total ?? 0)}
        >
          NEXT
        </Link>
      </div>
    </div>
  );
};

export const Route = createLazyFileRoute("/communities/$communityId/")({
  component: Community,
});

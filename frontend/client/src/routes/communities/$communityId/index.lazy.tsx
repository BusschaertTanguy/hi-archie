import { createLazyFileRoute, Link } from "@tanstack/react-router";
import {
  PostsEnumsPostVoteType,
  useDeleteApiV1CommunitiesId,
  useGetApiV1CommunitiesId,
  useGetApiV1Posts,
  usePostApiV1CommunitiesJoin,
  usePostApiV1CommunitiesLeave,
  usePostApiV1PostsVote,
} from "../../../api/types";
import Button from "../../../components/button.tsx";
import useUser from "../../../hooks/use-user.ts";
import Voter from "./-components/voter.tsx";

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

  const deleteMutation = useDeleteApiV1CommunitiesId({
    mutation: {
      onSuccess: () =>
        navigate({
          to: "/communities",
          search: { pageIndex: 0, pageSize: 25 },
        }),
    },
  });

  const voteMutation = usePostApiV1PostsVote({
    mutation: {
      onSuccess: () => postsQuery.refetch(),
    },
  });

  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-center justify-between">
        <div className="text-2xl">{communityQuery.data?.name}</div>
        <div className="flex gap-2">
          {userId && joinedCommunities?.includes(communityId) && (
            <>
              <Button
                color="black"
                variant="filled"
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
            </>
          )}
          {userId && !joinedCommunities?.includes(communityId) && (
            <Button
              color="black"
              variant="filled"
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
          {userId && communityQuery.data?.ownerId === userId && (
            <>
              <Button
                color="blue"
                variant="filled"
                onClick={() =>
                  navigate({
                    to: "/communities/$communityId/edit",
                    params: { communityId },
                  })
                }
              >
                EDIT
              </Button>
              <Button
                color="red"
                variant="filled"
                onClick={() => deleteMutation.mutateAsync({ id: communityId })}
              >
                REMOVE
              </Button>
            </>
          )}
        </div>
      </div>
      <div className="flex flex-col gap-4">
        {postsQuery.data?.posts.map((post) => (
          <div
            key={post.id}
            className="flex items-center justify-between rounded p-4 outline outline-1 outline-black"
          >
            <div className="flex items-center gap-4">
              <div
                className="text-lg hover:cursor-pointer"
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
              <div className="text-xs">
                {new Date(post.publishDate).toLocaleString()}
              </div>
            </div>
            <Voter
              votes={post.up - post.down}
              onUp={() =>
                voteMutation.mutateAsync({
                  data: {
                    postId: post.id,
                    type: PostsEnumsPostVoteType.Upvote,
                  },
                })
              }
              onDown={() =>
                voteMutation.mutateAsync({
                  data: {
                    postId: post.id,
                    type: PostsEnumsPostVoteType.Downvote,
                  },
                })
              }
            />
          </div>
        ))}
      </div>
      <div className="flex items-center justify-end gap-2">
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

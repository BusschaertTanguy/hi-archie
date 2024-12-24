import { createLazyFileRoute } from "@tanstack/react-router";
import {
  CommentsQueriesGetCommentsResponse,
  PostsEnumsPostVoteType,
  useDeleteApiV1PostsId,
  useGetApiV1Comments,
  useGetApiV1CommunitiesId,
  useGetApiV1PostsId,
  usePostApiV1Comments,
  usePostApiV1CommentsVote,
  usePostApiV1PostsVote,
  usePutApiV1Comments,
} from "../../../../../api/types";
import FormTextAreaInput from "../../../../../components/form-text-area-input.tsx";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import Button from "../../../../../components/button.tsx";
import useDisclosure from "../../../../../hooks/use-disclosure.ts";
import { useCallback, useEffect, useMemo, useState } from "react";
import Comment, { CommentNode } from "./-components/comment.tsx";
import useUser from "../../../../../hooks/use-user.ts";
import Voter from "../../-components/voter.tsx";

const schema = z.object({
  content: z.string().min(1).max(10000),
});

export type CommentSchema = z.infer<typeof schema>;
export interface CommentAction {
  readonly comment: CommentsQueriesGetCommentsResponse;
  readonly action: "reply" | "edit";
}

const buildCommentNode = (
  comment: CommentsQueriesGetCommentsResponse,
  comments: CommentsQueriesGetCommentsResponse[],
): CommentNode => {
  return {
    comment: comment,
    children: comments
      .filter((c) => c.parentId === comment.id)
      .map((c) => buildCommentNode(c, comments)),
  } satisfies CommentNode;
};

const Post = () => {
  const { userId } = useUser();
  const { communityId, postId } = Route.useParams();
  const navigate = Route.useNavigate();

  const {
    isOpen,
    handlers: { toggle },
  } = useDisclosure();

  const [commentAction, setCommentAction] = useState<CommentAction>();

  const form = useForm<CommentSchema>({
    resolver: zodResolver(schema),
  });

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
    reset,
  } = form;

  const communityQuery = useGetApiV1CommunitiesId(communityId);

  const postQuery = useGetApiV1PostsId(postId);

  const commentsQuery = useGetApiV1Comments({ postId });

  const onPostFormSuccess = useCallback(async () => {
    await commentsQuery.refetch();

    if (isOpen) {
      toggle();
    }

    if (commentAction) {
      setCommentAction(undefined);
    }

    reset();
  }, [commentAction, commentsQuery, isOpen, reset, toggle]);

  const addCommentMutation = usePostApiV1Comments({
    mutation: {
      onSuccess: onPostFormSuccess,
    },
  });

  const editCommentMutation = usePutApiV1Comments({
    mutation: {
      onSuccess: onPostFormSuccess,
    },
  });

  const removeMutation = useDeleteApiV1PostsId({
    mutation: {
      onSuccess: () =>
        navigate({
          to: "/communities/$communityId",
          params: { communityId },
          search: { pageIndex: 0, pageSize: 25 },
        }),
    },
  });

  const votePostMutation = usePostApiV1PostsVote({
    mutation: {
      onSuccess: () => postQuery.refetch(),
    },
  });

  const voteCommentMutation = usePostApiV1CommentsVote({
    mutation: {
      onSuccess: () => commentsQuery.refetch(),
    },
  });

  const onSubmitComment = useCallback(
    async (data: CommentSchema) => {
      if (!commentAction) {
        await addCommentMutation.mutateAsync({
          data: {
            id: crypto.randomUUID(),
            postId,
            content: data.content,
          },
        });
      }

      if (commentAction && commentAction.action === "reply") {
        await addCommentMutation.mutateAsync({
          data: {
            id: crypto.randomUUID(),
            postId,
            content: data.content,
            parentId: commentAction.comment.id,
          },
        });
      }

      if (commentAction && commentAction.action === "edit") {
        await editCommentMutation.mutateAsync({
          data: {
            id: commentAction.comment.id,
            content: data.content,
          },
        });
      }
    },
    [commentAction, addCommentMutation, postId, editCommentMutation],
  );

  const onCancelComment = useCallback(() => {
    reset();
    setCommentAction(undefined);
  }, [reset]);

  const onSelectComment = useCallback(
    (action: CommentAction) => {
      reset();
      setCommentAction(action);
    },
    [reset],
  );

  const parentNodes = useMemo(
    () =>
      commentsQuery.data
        ?.filter((c) => !c.parentId)
        .map((c) => buildCommentNode(c, commentsQuery.data)),
    [commentsQuery.data],
  );

  useEffect(() => {
    setValue(
      "content",
      !commentAction || commentAction.action === "reply"
        ? ""
        : commentAction.comment.content,
    );
  }, [commentAction, setValue]);

  return (
    <div className="flex flex-col gap-6">
      <div className="flex flex-col gap-2">
        <div
          className="text-sm hover:cursor-pointer"
          onClick={() =>
            navigate({
              to: "/communities/$communityId",
              params: { communityId },
              search: { pageIndex: 0, pageSize: 25 },
            })
          }
        >
          {communityQuery.data?.name}
        </div>
        <div className="flex justify-between">
          <div className="text-2xl">{postQuery.data?.title}</div>
          <div className="flex gap-2">
            {userId && (
              <Button color="black" variant="filled" onClick={toggle}>
                COMMENT
              </Button>
            )}
            {userId && postQuery.data?.ownerId === userId && (
              <>
                <Button
                  color="blue"
                  variant="filled"
                  onClick={() =>
                    navigate({
                      to: "/communities/$communityId/posts/$postId/edit",
                      params: { communityId, postId },
                    })
                  }
                >
                  EDIT
                </Button>
                <Button
                  color="red"
                  variant="filled"
                  onClick={() => removeMutation.mutateAsync({ id: postId })}
                >
                  REMOVE
                </Button>
              </>
            )}
          </div>
        </div>
        <div className="flex items-center gap-2">
          <Voter
            votes={(postQuery.data?.up ?? 0) - (postQuery.data?.down ?? 0)}
            onUp={() =>
              votePostMutation.mutateAsync({
                data: {
                  postId: postQuery.data?.id ?? "",
                  type: PostsEnumsPostVoteType.Upvote,
                },
              })
            }
            onDown={() =>
              votePostMutation.mutateAsync({
                data: {
                  postId: postQuery.data?.id ?? "",
                  type: PostsEnumsPostVoteType.Downvote,
                },
              })
            }
          />
          <div className="text-xs">
            {new Date(postQuery.data?.publishDate ?? "").toLocaleString()}
          </div>
        </div>
      </div>
      <div className="rounded border border-black p-4">
        {postQuery.data?.content}
      </div>

      {isOpen && (
        <form
          className="flex flex-col gap-4"
          onSubmit={handleSubmit(onSubmitComment)}
        >
          <FormTextAreaInput
            label="Add Comment"
            maxLength={10000}
            rows={10}
            className="resize-none"
            error={errors.content}
            {...register("content")}
          />
          <div className="flex items-center justify-end gap-2">
            <Button
              type="button"
              color="black"
              variant="outlined"
              onClick={() => {
                toggle();
                reset();
              }}
            >
              CANCEL
            </Button>
            <Button type="submit" color="black" variant="filled">
              SAVE
            </Button>
          </div>
        </form>
      )}
      <div className="flex flex-col gap-4">
        {parentNodes?.map((c) => (
          <Comment
            key={c.comment.id}
            node={c}
            form={form}
            commentAction={commentAction}
            onSubmitComment={onSubmitComment}
            onCancelComment={onCancelComment}
            onSelectComment={onSelectComment}
            onVote={(commentId, type) =>
              voteCommentMutation.mutateAsync({ data: { commentId, type } })
            }
          />
        ))}
      </div>
    </div>
  );
};

export const Route = createLazyFileRoute(
  "/communities/$communityId/posts/$postId/",
)({
  component: Post,
});

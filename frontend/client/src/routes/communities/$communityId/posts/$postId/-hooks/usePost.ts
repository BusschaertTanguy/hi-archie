import useUser from "../../../../../../hooks/use-user.ts";
import { useQueryClient } from "@tanstack/react-query";
import useDisclosure from "../../../../../../hooks/use-disclosure.ts";
import { useCallback, useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  CommentsEnumsCommentVoteType,
  CommentsProjectionsCommentProjection,
  CommentsQueriesGetCommentVotesResponse,
  useDeleteApiV1PostsId,
  useGetApiV1Comments,
  useGetApiV1CommentsVotes,
  useGetApiV1CommunitiesId,
  useGetApiV1PostsId,
  usePostApiV1Comments,
  usePostApiV1CommentsVote,
  usePostApiV1PostsVote,
  usePutApiV1Comments,
} from "../../../../../../api/types";
import recursiveSearch from "../../../../../../utils/recursive-search.ts";
import { z } from "zod";
import { useNavigate, useParams } from "@tanstack/react-router";

const schema = z.object({
  content: z.string().min(1).max(10000),
});

export type CommentSchema = z.infer<typeof schema>;

export interface CommentAction {
  readonly comment: CommentsProjectionsCommentProjection;
  readonly action: "reply" | "edit";
}

const usePost = () => {
  const { userId } = useUser();
  const queryClient = useQueryClient();

  const {
    isOpen,
    handlers: { toggle },
  } = useDisclosure();

  const [commentAction, setCommentAction] = useState<CommentAction>();

  const form = useForm<CommentSchema>({
    resolver: zodResolver(schema),
  });

  const { communityId, postId } = useParams({
    from: "/communities/$communityId/posts/$postId/",
  });

  const navigate = useNavigate({
    from: "/communities/$communityId/posts/$postId",
  });

  const navigateToCommunity = useCallback(
    () =>
      navigate({
        to: "/communities/$communityId",
        params: { communityId },
        search: { pageIndex: 0, pageSize: 25 },
      }),
    [communityId, navigate],
  );

  const navigateToEdit = useCallback(
    () =>
      navigate({
        to: "/communities/$communityId/posts/$postId/edit",
        params: { communityId, postId },
      }),
    [communityId, navigate, postId],
  );

  const communityQuery = useGetApiV1CommunitiesId(communityId);
  const postQuery = useGetApiV1PostsId(postId);
  const commentsQuery = useGetApiV1Comments({ postId });
  const commentVotesQuery = useGetApiV1CommentsVotes({ postId });

  const onPostFormSuccess = useCallback(() => {
    if (isOpen) {
      toggle();
    }

    if (commentAction) {
      setCommentAction(undefined);
    }

    form.reset();
  }, [commentAction, isOpen, form, toggle]);

  const addCommentMutation = usePostApiV1Comments({
    mutation: {
      onSuccess: (_, variables) => {
        const projection: CommentsProjectionsCommentProjection = {
          id: variables.data.id,
          content: variables.data.content,
          comments: [],
          publishDate: variables.data.publishDate,
          ownerId: userId ?? "",
          parentId: variables.data.parentId ?? "",
          up: 0,
          down: 0,
        };

        queryClient.setQueryData(commentsQuery.queryKey, (old) => {
          if (!old) {
            return old;
          }

          if (projection.parentId) {
            const parent = recursiveSearch(
              old,
              (old) => old.comments,
              (old) => old.id,
              projection.parentId,
            );

            parent?.comments.push(projection as never);
          } else {
            old = [...old, projection];
          }

          return old;
        });

        onPostFormSuccess();
      },
    },
  });

  const editCommentMutation = usePutApiV1Comments({
    mutation: {
      onSuccess: (_, variables) => {
        queryClient.setQueryData(commentsQuery.queryKey, (old) => {
          if (!old) {
            return old;
          }

          const oldComment = recursiveSearch(
            old,
            (old) => old.comments,
            (old) => old.id,
            variables.data.id,
          );

          if (oldComment) {
            oldComment.content = variables.data.content;
          }

          return old;
        });

        onPostFormSuccess();
      },
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
      onSuccess: (_, variables) => {
        const { commentId, type } = variables.data;

        const oldType = queryClient
          .getQueryData(commentVotesQuery.queryKey)
          ?.find((cv) => cv.commentId === commentId)?.type;

        const newVotes = queryClient.setQueryData(
          commentVotesQuery.queryKey,
          (old) => {
            if (!old) {
              return old;
            }

            const existing = old.find((cv) => cv.commentId === commentId);

            if (!existing) {
              const next: CommentsQueriesGetCommentVotesResponse = {
                commentId,
                type,
              };

              return [...old, next];
            }

            if (existing.type === type) {
              return old.filter((cv) => cv.commentId !== commentId);
            }

            existing.type = type;
            return old;
          },
        );

        queryClient.setQueryData(commentsQuery.queryKey, (old) => {
          if (!old || !newVotes) {
            return old;
          }

          const existingComment = recursiveSearch(
            old,
            (old) => old.comments,
            (old) => old.id,
            commentId,
          );

          if (!existingComment) {
            return old;
          }

          const existingVote = newVotes.find(
            (cv) => cv.commentId === existingComment.id,
          );

          let upChange = 0;
          let downChange = 0;

          if (!existingVote) {
            if (type === CommentsEnumsCommentVoteType.Upvote) {
              upChange--;
            }

            if (type === CommentsEnumsCommentVoteType.Downvote) {
              downChange--;
            }
          } else {
            if (existingVote.type === CommentsEnumsCommentVoteType.Upvote) {
              upChange++;
            }

            if (existingVote.type === CommentsEnumsCommentVoteType.Downvote) {
              downChange++;
            }
          }

          if (oldType && oldType !== existingVote?.type) {
            if (existingVote?.type === CommentsEnumsCommentVoteType.Upvote) {
              downChange--;
            }

            if (existingVote?.type === CommentsEnumsCommentVoteType.Downvote) {
              upChange--;
            }
          }

          existingComment.up += upChange;
          existingComment.down += downChange;

          return old;
        });
      },
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
            publishDate: new Date().toISOString(),
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
            publishDate: new Date().toISOString(),
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
    form.reset();
    setCommentAction(undefined);
  }, [form]);

  const onSelectComment = useCallback(
    (action: CommentAction) => {
      form.reset();
      setCommentAction(action);
    },
    [form],
  );

  useEffect(() => {
    form.setValue(
      "content",
      !commentAction || commentAction.action === "reply"
        ? ""
        : commentAction.comment.content,
    );
  }, [commentAction, form]);

  return {
    userId,
    navigateToCommunity,
    navigateToEdit,
    form,
    handleSubmit: form.handleSubmit(onSubmitComment),
    isOpen,
    toggle,
    communityQuery,
    postQuery,
    commentsQuery,
    commentVotesQuery,
    removeMutation: () => removeMutation.mutateAsync({ id: postId }),
    votePostMutation,
    voteCommentMutation,
    onSubmitComment,
    onSelectComment,
    onCancelComment,
    commentAction,
  };
};

export default usePost;

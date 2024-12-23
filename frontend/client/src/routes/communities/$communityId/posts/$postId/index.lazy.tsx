import { createLazyFileRoute } from "@tanstack/react-router";
import {
  CommentsQueriesGetCommentsResponse,
  useGetApiV1Comments,
  useGetApiV1PostsId,
  usePostApiV1Comments,
} from "../../../../../api/types";
import FormTextAreaInput from "../../../../../components/form-text-area-input.tsx";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import Button from "../../../../../components/button.tsx";
import useDisclosure from "../../../../../hooks/use-disclosure.ts";
import { useCallback, useMemo, useState } from "react";
import Comment, { CommentNode } from "./-components/comment.tsx";

const schema = z.object({
  content: z.string().min(1).max(10000),
});

export type CommentReplySchema = z.infer<typeof schema>;

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
  const { postId } = Route.useParams();

  const {
    isOpen,
    handlers: { toggle },
  } = useDisclosure();

  const [replyId, setReplyId] = useState<string>();

  const form = useForm<CommentReplySchema>({
    resolver: zodResolver(schema),
  });

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = form;

  const postQuery = useGetApiV1PostsId(postId);

  const commentsQuery = useGetApiV1Comments({ postId });

  const addCommentMutation = usePostApiV1Comments({
    mutation: {
      onSuccess: async () => {
        await commentsQuery.refetch();

        if (isOpen) {
          toggle();
        }

        if (replyId) {
          setReplyId(undefined);
        }

        reset();
      },
    },
  });

  const onSubmit = useCallback(
    async (data: CommentReplySchema) => {
      await addCommentMutation.mutateAsync({
        data: {
          id: crypto.randomUUID(),
          postId,
          content: data.content,
          parentId: replyId,
        },
      });

      reset();
    },
    [addCommentMutation, postId, replyId, reset],
  );

  const onCancelReply = useCallback(() => {
    setReplyId(undefined);
    reset();
  }, [reset]);

  const onSelectReply = useCallback((replyId: string) => {
    setReplyId(replyId);
  }, []);

  const parentNodes = useMemo(
    () =>
      commentsQuery.data
        ?.filter((c) => !c.parentId)
        .map((c) => buildCommentNode(c, commentsQuery.data)),
    [commentsQuery.data],
  );

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <div className="flex flex-col gap-2">
          <div className="text-2xl">{postQuery.data?.title}</div>
          <div className="text-xs">
            {new Date(postQuery.data?.publishDate ?? "").toLocaleString()}
          </div>
        </div>
        <Button color="black" variant="filled" onClick={toggle}>
          COMMENT
        </Button>
      </div>
      <div className="rounded border border-black p-3">
        {postQuery.data?.content}
      </div>
      {isOpen && (
        <form className="flex flex-col gap-6" onSubmit={handleSubmit(onSubmit)}>
          <FormTextAreaInput
            label="Add Comment"
            maxLength={10000}
            rows={10}
            className="resize-none"
            error={errors.content}
            {...register("content")}
          />
          <div className="flex items-center justify-end gap-4">
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
            selectedReply={replyId}
            onSubmitReply={onSubmit}
            onCancelReply={onCancelReply}
            onSelectReply={onSelectReply}
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

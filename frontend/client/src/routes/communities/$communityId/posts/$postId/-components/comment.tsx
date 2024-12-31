import { memo, useMemo } from "react";
import Button from "../../../../../../components/button.tsx";
import FormTextAreaInput from "../../../../../../components/form-text-area-input.tsx";
import {
  CommentsEnumsCommentVoteType,
  CommentsProjectionsCommentProjection,
  CommentsQueriesGetCommentVotesResponse,
} from "../../../../../../api/types";
import { UseFormReturn } from "react-hook-form";
import Voter from "../../../-components/voter.tsx";
import { CommentAction, CommentSchema } from "../-hooks/usePost.ts";

interface CommentProps {
  readonly comment: CommentsProjectionsCommentProjection;
  readonly form: UseFormReturn<CommentSchema>;
  readonly commentAction?: CommentAction;
  readonly commentVotes?: CommentsQueriesGetCommentVotesResponse[];
  readonly onSubmitComment: (data: CommentSchema) => Promise<void>;
  readonly onCancelComment: () => void;
  readonly onSelectComment: (commentAction: CommentAction) => void;
  readonly onVote: (
    commentId: string,
    type: CommentsEnumsCommentVoteType,
  ) => Promise<void>;
}

const Comment = ({
  comment,
  form,
  commentAction,
  commentVotes,
  onSubmitComment,
  onSelectComment,
  onCancelComment,
  onVote,
}: CommentProps) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = form;

  const { id, content, comments, ownerId, up, down, publishDate } = comment;

  const vote = useMemo(
    () => commentVotes?.find((cv) => cv.commentId === id),
    [commentVotes, id],
  );

  return (
    <div className="flex flex-col gap-2">
      <div className="flex flex-col justify-around gap-2">
        <div className="flex gap-1 text-xs">
          <span className="font-semibold">User: {ownerId}</span>
          <span className="text-slate-500">
            {new Date(publishDate).toLocaleString()}
          </span>
        </div>
        <div>{content}</div>
        <div className="flex gap-1.5">
          <Voter
            votes={up - down}
            onUp={() => onVote(id, CommentsEnumsCommentVoteType.Upvote)}
            onDown={() => onVote(id, CommentsEnumsCommentVoteType.Downvote)}
            selectedType={vote?.type}
          />
          <span
            className="text-xs text-slate-500 hover:cursor-pointer hover:underline"
            onClick={() => {
              if (
                commentAction?.action === "reply" &&
                commentAction.comment.id === id
              ) {
                onCancelComment();
              } else {
                onSelectComment({ comment: comment, action: "reply" });
              }
            }}
          >
            Reply
          </span>
          <span
            className="text-xs text-slate-500 hover:cursor-pointer hover:underline"
            onClick={() => {
              if (
                commentAction?.action === "edit" &&
                commentAction.comment.id === id
              ) {
                onCancelComment();
              } else {
                onSelectComment({ comment: comment, action: "edit" });
              }
            }}
          >
            Edit
          </span>
        </div>
      </div>
      {commentAction?.comment.id === id && (
        <form
          className="flex flex-col gap-6 pt-3"
          onSubmit={handleSubmit(onSubmitComment)}
        >
          <FormTextAreaInput
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
              onClick={onCancelComment}
            >
              CANCEL
            </Button>
            <Button type="submit" color="black" variant="filled">
              SAVE
            </Button>
          </div>
        </form>
      )}
      <div className="flex flex-col gap-3 border-l border-slate-300 pl-8">
        {comments.map((c) => (
          <Comment
            key={c.id}
            comment={c}
            form={form}
            commentAction={commentAction}
            commentVotes={commentVotes}
            onSubmitComment={onSubmitComment}
            onCancelComment={onCancelComment}
            onSelectComment={onSelectComment}
            onVote={onVote}
          />
        ))}
      </div>
    </div>
  );
};

export default memo(Comment);

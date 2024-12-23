import { memo } from "react";
import Button from "../../../../../../components/button.tsx";
import FormTextAreaInput from "../../../../../../components/form-text-area-input.tsx";
import { CommentsQueriesGetCommentsResponse } from "../../../../../../api/types";
import { UseFormReturn } from "react-hook-form";
import { CommentAction, CommentSchema } from "../index.lazy.tsx";

export interface CommentNode {
  readonly children?: CommentNode[];
  readonly comment: CommentsQueriesGetCommentsResponse;
}

interface CommentProps {
  readonly node: CommentNode;
  readonly form: UseFormReturn<CommentSchema>;
  readonly commentAction?: CommentAction;
  readonly onSubmitComment: (data: CommentSchema) => Promise<void>;
  readonly onCancelComment: () => void;
  readonly onSelectComment: (commentAction: CommentAction) => void;
}

const Comment = ({
  node,
  form,
  commentAction,
  onSubmitComment,
  onSelectComment,
  onCancelComment,
}: CommentProps) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = form;

  const { comment, children } = node;

  return (
    <div className="flex flex-col gap-2">
      <div className="flex flex-col justify-around gap-2">
        <div className="flex gap-1 text-xs">
          <span className="font-semibold">User: {comment.ownerId}</span>
          <span className="text-slate-500">
            {new Date(comment.publishDate).toLocaleString()}
          </span>
        </div>
        <div>{comment.content}</div>
        <div className="flex gap-1.5">
          <span
            className="text-xs text-slate-500 hover:cursor-pointer hover:underline"
            onClick={() => {
              if (
                commentAction?.action === "reply" &&
                commentAction.comment.id === comment.id
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
                commentAction.comment.id === comment.id
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
      {commentAction?.comment.id === comment.id && (
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
        {children?.map((childNode) => (
          <Comment
            key={childNode.comment.id}
            node={childNode}
            form={form}
            commentAction={commentAction}
            onSubmitComment={onSubmitComment}
            onCancelComment={onCancelComment}
            onSelectComment={onSelectComment}
          />
        ))}
      </div>
    </div>
  );
};

export default memo(Comment);

import { memo } from "react";
import Button from "../../../../../../components/button.tsx";
import FormTextAreaInput from "../../../../../../components/form-text-area-input.tsx";
import { CommentsQueriesGetCommentsResponse } from "../../../../../../api/types";
import { UseFormReturn } from "react-hook-form";
import { CommentReplySchema } from "../index.lazy.tsx";

export interface CommentNode {
  readonly children?: CommentNode[];
  readonly comment: CommentsQueriesGetCommentsResponse;
}

interface CommentProps {
  readonly node: CommentNode;
  readonly form: UseFormReturn<CommentReplySchema>;
  readonly selectedReply?: string;
  readonly onSubmitReply: (data: CommentReplySchema) => Promise<void>;
  readonly onCancelReply: () => void;
  readonly onSelectReply: (replyId: string) => void;
}

const Comment = ({
  node,
  form,
  selectedReply,
  onSubmitReply,
  onSelectReply,
  onCancelReply,
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
        <span
          className="text-xs text-slate-500 hover:cursor-pointer hover:underline"
          onClick={() => {
            if (selectedReply) {
              onCancelReply();
            } else {
              onSelectReply(comment.id);
            }
          }}
        >
          Reply
        </span>
      </div>
      {selectedReply === comment.id && (
        <form
          className="flex flex-col gap-6 border-t border-slate-300 pt-3"
          onSubmit={handleSubmit(onSubmitReply)}
        >
          <FormTextAreaInput
            label="Reply"
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
              onClick={onCancelReply}
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
            selectedReply={selectedReply}
            onSubmitReply={onSubmitReply}
            onCancelReply={onCancelReply}
            onSelectReply={onSelectReply}
          />
        ))}
      </div>
    </div>
  );
};

export default memo(Comment);

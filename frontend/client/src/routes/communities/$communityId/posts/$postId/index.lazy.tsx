import { createLazyFileRoute } from "@tanstack/react-router";
import { PostsEnumsPostVoteType } from "../../../../../api/types";
import FormTextAreaInput from "../../../../../components/form-text-area-input.tsx";
import Button from "../../../../../components/button.tsx";
import Comment from "./-components/comment.tsx";
import Voter from "../../-components/voter.tsx";
import usePost from "./-hooks/usePost.ts";

const Post = () => {
  const {
    userId,
    navigateToCommunity,
    navigateToEdit,
    form,
    handleSubmit,
    isOpen,
    toggle,
    communityQuery,
    postQuery,
    commentsQuery,
    commentVotesQuery,
    removeMutation,
    votePostMutation,
    voteCommentMutation,
    onSubmitComment,
    onCancelComment,
    onSelectComment,
    commentAction,
  } = usePost();

  return (
    <div className="flex flex-col gap-6">
      <div className="flex flex-col gap-2">
        <div
          className="text-sm hover:cursor-pointer"
          onClick={navigateToCommunity}
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
                <Button color="blue" variant="filled" onClick={navigateToEdit}>
                  EDIT
                </Button>
                <Button color="red" variant="filled" onClick={removeMutation}>
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
            selectedType={postQuery.data?.currentVote}
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
        <form className="flex flex-col gap-4" onSubmit={handleSubmit}>
          <FormTextAreaInput
            label="Add Comment"
            maxLength={10000}
            rows={10}
            className="resize-none"
            error={form.formState.errors.content}
            {...form.register("content")}
          />
          <div className="flex items-center justify-end gap-2">
            <Button
              type="button"
              color="black"
              variant="outlined"
              onClick={() => {
                toggle();
                form.reset();
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
        {commentsQuery.data?.map((c) => (
          <Comment
            key={c.id}
            comment={c}
            form={form}
            commentAction={commentAction}
            commentVotes={commentVotesQuery.data}
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

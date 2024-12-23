import { createLazyFileRoute, Navigate } from "@tanstack/react-router";
import FormTextInput from "../../../../../components/form-text-input.tsx";
import FormTextAreaInput from "../../../../../components/form-text-area-input.tsx";
import Button from "../../../../../components/button.tsx";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { usePostApiV1Posts } from "../../../../../api/types";
import useUser from "../../../../../hooks/use-user.ts";
import { useCallback } from "react";

const schema = z.object({
  title: z.string().min(1).max(50),
  content: z.string().min(1).max(10000),
});

type Schema = z.infer<typeof schema>;

const AddPost = () => {
  const { joinedCommunities } = useUser();
  const addPostMutation = usePostApiV1Posts();
  const { communityId } = Route.useParams();
  const navigate = Route.useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<Schema>({
    resolver: zodResolver(schema),
  });

  const onSubmit = useCallback(
    async (data: Schema) => {
      await addPostMutation.mutateAsync({
        data: {
          id: crypto.randomUUID(),
          communityId,
          ...data,
        },
      });

      await navigate({ to: ".." });
    },
    [addPostMutation, communityId, navigate],
  );

  if (!joinedCommunities?.includes(communityId)) {
    return <Navigate to={".."} />;
  }

  return (
    <form className="flex flex-col gap-6" onSubmit={handleSubmit(onSubmit)}>
      <FormTextInput
        label="Title"
        error={errors.title}
        {...register("title")}
      />
      <FormTextAreaInput
        label="Content"
        maxLength={10000}
        rows={10}
        className="resize-none"
        error={errors.content}
        {...register("content")}
      />
      <div className="flex justify-end gap-4">
        <Button
          color={"black"}
          variant={"outlined"}
          type="button"
          onClick={() => navigate({ to: ".." })}
        >
          Cancel
        </Button>
        <Button color={"black"} variant={"filled"} type="submit">
          Save
        </Button>
      </div>
    </form>
  );
};

export const Route = createLazyFileRoute(
  "/communities/$communityId/posts/add/",
)({
  component: AddPost,
});
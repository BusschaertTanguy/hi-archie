import { memo } from "react";
import { PostsQueriesGetPostResponse } from "../../../../../api/types";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import FormTextInput from "../../../../../components/form-text-input.tsx";
import FormTextAreaInput from "../../../../../components/form-text-area-input.tsx";
import Button from "../../../../../components/button.tsx";

interface PostFormProps {
  readonly post?: PostsQueriesGetPostResponse;
  readonly onCancel: () => void;
  readonly onSubmit: (data: Schema) => Promise<void>;
}

const schema = z.object({
  title: z.string().min(1).max(50),
  content: z.string().min(1).max(10000),
});

type Schema = z.infer<typeof schema>;

const PostForm = ({ post, onCancel, onSubmit }: PostFormProps) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<Schema>({
    defaultValues: {
      ...post,
    },
    resolver: zodResolver(schema),
  });

  return (
    <div className="flex flex-col gap-6">
      <div className="text-xl">{post ? "Edit" : "Add"} Post</div>
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
            onClick={onCancel}
          >
            Cancel
          </Button>
          <Button color={"black"} variant={"filled"} type="submit">
            Save
          </Button>
        </div>
      </form>
    </div>
  );
};

export default memo(PostForm);

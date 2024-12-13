import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { usePostApiV1Communities } from "../../../api/types";
import Button from "../../../components/button.tsx";

export interface AddCommunityProps {
  readonly onCancel?: () => void;
  readonly onSave?: () => void;
}

const schema = z.object({
  name: z.string().min(1).max(50),
});

type Schema = z.infer<typeof schema>;

const AddCommunity = ({ onCancel, onSave }: AddCommunityProps) => {
  const addCommunityMutation = usePostApiV1Communities({
    mutation: { onSuccess: onSave },
  });

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<Schema>({
    resolver: zodResolver(schema),
  });

  const onSubmit = async (data: Schema) => {
    await addCommunityMutation.mutateAsync({
      data: {
        id: crypto.randomUUID(),
        name: data.name,
      },
    });
  };

  return (
    <div className="flex flex-col gap-6">
      <div className="text-xl">Add Community</div>
      <form id="add-community-form" onSubmit={handleSubmit(onSubmit)}>
        <div className="flex flex-col gap-2">
          <label>Name</label>
          <input
            className="rounded px-2 py-1 outline outline-1"
            {...register("name")}
          />
          {errors.name && <p className="text-red-500">{errors.name.message}</p>}
        </div>
      </form>
      <div className="flex justify-end gap-4">
        <Button variant="outlined" color="red" onClick={onCancel}>
          Cancel
        </Button>
        <Button
          variant="filled"
          color="blue"
          type="submit"
          form="add-community-form"
        >
          Save
        </Button>
      </div>
    </div>
  );
};

export default AddCommunity;

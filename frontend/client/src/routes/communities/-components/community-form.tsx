import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { CommunitiesQueriesGetCommunityResponse } from "../../../api/types";
import Button from "../../../components/button.tsx";
import FormTextInput from "../../../components/form-text-input.tsx";
import { memo } from "react";

export interface CommunityFormProps {
  readonly community?: CommunitiesQueriesGetCommunityResponse;
  readonly onCancel: () => void;
  readonly onSubmit: (data: Schema) => Promise<void>;
}

const schema = z.object({
  name: z.string().min(1).max(50),
});

type Schema = z.infer<typeof schema>;

const CommunityForm = ({
  community,
  onCancel,
  onSubmit,
}: CommunityFormProps) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<Schema>({
    defaultValues: {
      ...community,
    },
    resolver: zodResolver(schema),
  });

  return (
    <div className="flex flex-col gap-6">
      <div className="text-xl">{community ? "Edit" : "Add"} Community</div>
      <form id="add-community-form" onSubmit={handleSubmit(onSubmit)}>
        <FormTextInput
          label="Name"
          className="w-72"
          error={errors.name}
          {...register("name")}
        />
      </form>
      <div className="flex justify-end gap-4">
        <Button variant="outlined" color="black" onClick={onCancel}>
          Cancel
        </Button>
        <Button
          variant="filled"
          color="black"
          type="submit"
          form="add-community-form"
        >
          Save
        </Button>
      </div>
    </div>
  );
};

export default memo(CommunityForm);

import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  CommunitiesQueriesGetCommunitiesDto,
  usePostApiV1Communities,
  usePutApiV1Communities,
} from "../../../api/types";
import Button from "../../../components/button.tsx";
import FormTextInput from "../../../components/form-text-input.tsx";

export interface CommunityFormProps {
  readonly community?: CommunitiesQueriesGetCommunitiesDto;
  readonly onCancel?: () => void;
  readonly onSave?: () => void;
}

const schema = z.object({
  name: z.string().min(1).max(50),
});

type Schema = z.infer<typeof schema>;

const CommunityForm = ({ community, onCancel, onSave }: CommunityFormProps) => {
  const addCommunityMutation = usePostApiV1Communities({
    mutation: { onSuccess: onSave },
  });

  const editCommunityMutation = usePutApiV1Communities({
    mutation: { onSuccess: onSave },
  });

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

  const onSubmit = async (data: Schema) => {
    if (community) {
      await editCommunityMutation.mutateAsync({
        data: {
          id: community.id,
          ...data,
        },
      });
    } else {
      await addCommunityMutation.mutateAsync({
        data: {
          id: crypto.randomUUID(),
          ...data,
        },
      });
    }
  };

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

export default CommunityForm;

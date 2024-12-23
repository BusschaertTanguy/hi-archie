import {
  ClassAttributes,
  forwardRef,
  memo,
  TextareaHTMLAttributes,
} from "react";
import { twMerge } from "tailwind-merge";
import { FieldError } from "react-hook-form";

interface FormInputProps
  extends ClassAttributes<HTMLTextAreaElement>,
    TextareaHTMLAttributes<HTMLTextAreaElement> {
  readonly label?: string;
  readonly error?: FieldError;
}

const FormTextAreaInput = forwardRef<HTMLTextAreaElement, FormInputProps>(
  ({ label, error, ...props }: FormInputProps, ref) => {
    const className = twMerge(
      "rounded px-2 py-1 outline outline-1",
      props.className,
    );

    return (
      <div className="flex flex-col gap-2">
        {label && <label htmlFor={props.id}>{label}</label>}
        <textarea {...props} className={className} ref={ref} />
        {error && <p className="text-red-500">{error.message}</p>}
      </div>
    );
  },
);

FormTextAreaInput.displayName = "FormTextAreaInput";

export default memo(FormTextAreaInput);

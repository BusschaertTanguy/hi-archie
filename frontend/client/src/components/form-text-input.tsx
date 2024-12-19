import { ClassAttributes, InputHTMLAttributes } from "react";
import { twMerge } from "tailwind-merge";
import { FieldError } from "react-hook-form";

interface FormInputProps
  extends ClassAttributes<HTMLInputElement>,
    InputHTMLAttributes<HTMLInputElement> {
  readonly label: string;
  readonly error?: FieldError;
}

const FormTextInput = ({ label, error, ...props }: FormInputProps) => {
  const className = twMerge(
    "rounded px-2 py-1 outline outline-1",
    props.className,
  );

  return (
    <div className="flex flex-col gap-2">
      <label htmlFor={props.id}>{label}</label>
      <input {...props} className={className} />
      {error && <p className="text-red-500">{error.message}</p>}
    </div>
  );
};

export default FormTextInput;

import { ButtonHTMLAttributes, ClassAttributes, memo } from "react";
import { twMerge } from "tailwind-merge";

type ButtonVariant = "filled" | "outlined";
type ButtonColor = "black" | "green" | "blue" | "red";

interface ButtonProps
  extends ClassAttributes<HTMLButtonElement>,
    ButtonHTMLAttributes<HTMLButtonElement> {
  readonly color: ButtonColor;
  readonly variant: ButtonVariant;
}

const handleColorVariant = (
  color: ButtonColor,
  variant: ButtonVariant,
  className?: string,
) => {
  if (color === "black" && variant === "filled") {
    return twMerge("bg-black text-white outline-black", className);
  }

  if (color === "black" && variant === "outlined") {
    return twMerge("text-black outline-black", className);
  }

  if (color === "green" && variant === "filled") {
    return twMerge("bg-green-500 text-green-50 outline-green-500", className);
  }

  if (color === "green" && variant === "outlined") {
    return twMerge("text-green-500 outline-green-500", className);
  }

  if (color === "blue" && variant === "filled") {
    return twMerge("bg-blue-500 text-blue-50 outline-blue-500", className);
  }

  if (color === "blue" && variant === "outlined") {
    return twMerge("text-blue-500 outline-blue-500", className);
  }

  if (color === "red" && variant === "filled") {
    return twMerge("bg-red-500 text-red-50 outline-red-500", className);
  }

  if (color === "red" && variant === "outlined") {
    return twMerge("text-red-500 outline-red-500", className);
  }
};

const Button = ({ children, color, variant, ...props }: ButtonProps) => {
  const className = twMerge(
    "rounded px-3 py-1 outline outline-1",
    handleColorVariant(color, variant),
    props.className,
  );

  return (
    <button {...props} className={className}>
      {children}
    </button>
  );
};

export default memo(Button);

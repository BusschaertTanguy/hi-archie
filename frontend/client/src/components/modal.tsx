import { PropsWithChildren, useEffect, useRef } from "react";

export interface ModalProps {
  readonly onClose?: () => void;
}

const Modal = ({ onClose, children }: PropsWithChildren<ModalProps>) => {
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleClick = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) {
        onClose?.();
      }
    };

    document.addEventListener("mousedown", handleClick);

    return () => {
      document.removeEventListener("mousedown", handleClick);
    };
  }, [onClose, ref]);

  return (
    <>
      <div className="fixed left-0 top-0 z-10 h-screen w-screen bg-black opacity-50"></div>
      <div className="fixed left-0 top-0 z-10 flex h-screen w-screen items-center justify-center">
        <div className="rounded bg-white px-12 py-10" ref={ref}>
          {children}
        </div>
      </div>
    </>
  );
};

export default Modal;

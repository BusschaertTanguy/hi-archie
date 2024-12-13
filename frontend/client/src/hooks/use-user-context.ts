import { UserContext } from "../contexts/user-context.ts";
import { use } from "react";

const useUserContext = () => {
  const context = use(UserContext);

  if (!context) {
    throw new Error("useUserContext must be used within the UserContext");
  }

  return context;
};

export default useUserContext;

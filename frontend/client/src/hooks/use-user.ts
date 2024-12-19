import { UserContext } from "../contexts/user-context.ts";
import { use } from "react";

const useUser = () => {
  const context = use(UserContext);

  if (!context) {
    throw new Error("useUser must be used within the UserContext");
  }

  return context;
};

export default useUser;

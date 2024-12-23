import { UserContext } from "../contexts/user-context.ts";
import { useContext } from "react";

const useUser = () => {
  const context = useContext(UserContext);

  if (!context) {
    throw new Error("useUser must be used within the UserContext");
  }

  return context;
};

export default useUser;

import { createContext } from "react";

export interface UserContextValue {
  readonly userId?: string;
}

export const UserContext = createContext<UserContextValue | null>(null);

import { createContext } from "react";

export interface UserContextValue {
  readonly id?: string;
}

export const UserContext = createContext<UserContextValue | null>(null);

import { createContext } from "react";

export interface UserContextValue {
  readonly userId?: string;
  readonly joinedCommunities?: string[];
  readonly refresh: () => void;
}

export const UserContext = createContext<UserContextValue | null>(null);

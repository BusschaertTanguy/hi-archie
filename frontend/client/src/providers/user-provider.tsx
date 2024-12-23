import { PropsWithChildren, useMemo } from "react";
import { useGetApiV1UsersMe } from "../api/types";
import { useAuth0 } from "@auth0/auth0-react";
import { UserContext, UserContextValue } from "../contexts/user-context.ts";

const UserProvider = ({ children }: PropsWithChildren) => {
  const { isAuthenticated, user } = useAuth0();

  const { data, refetch } = useGetApiV1UsersMe({
    query: { enabled: isAuthenticated && !!user },
  });

  const value = useMemo(
    () =>
      ({
        userId: data?.id,
        joinedCommunities: data?.joinedCommunities,
        refresh: refetch,
      }) satisfies UserContextValue,
    [data?.id, data?.joinedCommunities, refetch],
  );

  return <UserContext.Provider value={value}>{children}</UserContext.Provider>;
};

export default UserProvider;

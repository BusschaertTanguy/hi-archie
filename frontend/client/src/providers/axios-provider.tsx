import { PropsWithChildren, useEffect } from "react";
import addTokenInterceptor from "../api/interceptors/add-token-interceptor.ts";
import { useAuth0 } from "@auth0/auth0-react";

const AxiosProvider = ({ children }: PropsWithChildren) => {
  const { getAccessTokenSilently } = useAuth0();

  useEffect(() => {
    addTokenInterceptor(getAccessTokenSilently);
  }, [getAccessTokenSilently]);

  return <>{children}</>;
};

export default AxiosProvider;

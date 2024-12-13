import { Auth0ProviderOptions } from "@auth0/auth0-react/src/auth0-provider.tsx";

export const authConfig: Auth0ProviderOptions = {
  domain: import.meta.env.VITE_DOMAIN,
  clientId: import.meta.env.VITE_CLIENT_ID,
  authorizationParams: {
    redirect_uri: window.location.origin,
    audience: import.meta.env.VITE_AUDIENCE,
  },
};

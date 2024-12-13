import { AXIOS_INSTANCE } from "../mutator/axios-instance.ts";

const addTokenInterceptor = (getAccessToken: () => Promise<string>) => {
  AXIOS_INSTANCE.interceptors.request.use(async (config) => {
    try {
      const accessToken = await getAccessToken();

      if (accessToken) {
        config.headers.Authorization = `Bearer ${accessToken}`;
      }
    } catch {
      // Ignore get access token
    }

    return config;
  });
};

export default addTokenInterceptor;

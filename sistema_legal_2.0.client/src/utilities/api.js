import axios from "axios";
import store from "@/store";
import router from "../router/router";
import { push } from "notivue";

const baseURL = import.meta.env.BASE_URL;

const api = axios.create(
  {
    baseURL: baseURL
  }
);

api.interceptors.request.use((config) => {
  const requireLoading = config.requireLoading !== false;
  if (requireLoading) {
    store.commit('setLoading', true);
  }

  const token = localStorage.getItem("token");

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

api.interceptors.response.use(
  (response) => {
    store.commit('setLoading', false);
    return response;
  },
  (error) => {
    store.commit('setLoading', false);
    if (error.response.status == 400) {
      return Promise.resolve(error.response);
    }
    if (error.response.status == 401 && localStorage.getItem("token")) {
      localStorage.removeItem("token");
      localStorage.removeItem('sessionExpireTime');
      store.commit('setScreenLocked', true);
      push.warning("Debe iniciar sesión para continuar");
    }
    else if (error.response.status == 401) {
      localStorage.removeItem("token");
      localStorage.removeItem('user');
      localStorage.removeItem('sessionExpireTime');
      store.commit('setScreenLocked', false);
      router.push({ name: "Login" });
    }
    else if (error.response.status == 403) {
      push.error("No tienes permisos para realizar esta acción");
    }
    else {
      push.error(error);
    }
    return Promise.reject(error);
  }
);

export default api;

import axios, { AxiosInstance, AxiosResponse } from "axios";
import vueInstance from "@/main";
import { Dictionary } from "vue-router/types/router";

const instance: AxiosInstance = axios.create({
  baseURL: process.env.VUE_APP_SERVICE_URL,
});
instance.interceptors.response.use(
  (response: AxiosResponse) => {
    axios.defaults.headers.common["Access-Control-Allow-Origin"] = "*";
    console.log(response);

    // Si hay un token en la respuesta actualiza el Store
    if (response.data.token) {
      sessionStorage.setItem("token", response.data.token);
      axios.defaults.headers.common.Authorization = response.data.token;
    }
    // VueInstance.$toast.success("Yes");

    // No hubo error en la respuesta
    return response;
  },
  async (error) => {
    switch (error.response.status) {
      case 500:
        vueInstance.$toast.error("Error en el servidor. Pruebe mas tarde");
        return "Error en el servidor. Pruebe mas tarde";
      case 400:
        return vueInstance.$toast.error(error.response.data[0]);
      case 401:
        vueInstance.$toast.error("Usuario o contraseña incorrectos");
        vueInstance.$router.push("/login");
        return "Usuario o contraseña incorrectos";
      case 422:
        return vueInstance.$toast.error("Error al rellenar el formulario");
      default:
        return Promise.reject(error);
    }
    // VueInstance.$toast.error("No");
  }
);

export default instance;

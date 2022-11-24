import Vue from "vue";
import Vuex from "vuex";
import axios from "axios";

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    // drawer
    visibility: true,

    // licencia
    subscriptor: sessionStorage.getItem("subscriptor") || null,
    vencimiento: sessionStorage.getItem("vencimiento") || null,

    // auth
    status: sessionStorage.getItem("token") ? "success" : "",
    token: sessionStorage.getItem("token") || null,
    // @ts-ignore
    usuario: JSON.parse(sessionStorage.getItem("user_data")) || {
      nombre: "",
      roles: [],
    },
  },
  mutations: {
    // drawer
    show(state) {
      state.status = "visible";
      state.visibility = true;
    },
    hide(state) {
      state.status = "invisible";
      state.visibility = false;
    },

    // licencia
    quitar(state) {
      state.subscriptor = null;
      state.vencimiento = null;
    },
    agregar(state, data) {
      state.subscriptor = data.subscriptor;
      state.vencimiento = data.vencimiento;
    },
    // auth
    auth_request(state) {
      state.status = "loading";
    },
    auth_success(state, payload) {
      state.status = "success";
      state.token = payload.token;
      state.usuario = payload.usuario;
    },
    auth_error(state) {
      state.status = "error";
    },
    logout(state) {
      state.status = "";
      state.token = null;
      state.usuario = {
        nombre: "",
        roles: [],
      };
    },
  },
  actions: {
    // drawer
    changeVisibility({ commit }, visibility) {
      return new Promise<void>((resolve) => {
        if (visibility) {
          commit("show");
        } else {
          commit("hide");
        }
        resolve();
      });
    },

    // licencia
    agregar({ commit }, licence) {
      return new Promise((resolve) => {
        const lic = licence;
        commit("agregar", {
          subscriptor: lic.subscriptor,
          vencimiento: lic.fechaVencimiento,
        });
        sessionStorage.setItem("subscriptor", lic.subscriptor);
        sessionStorage.setItem("vencimiento", lic.fechaVencimiento);
        resolve(resolve);
      });
    },
    quitar({ commit }) {
      return new Promise((resolve, reject) => {
        const url = `${process.env.VUE_APP_SERVICE_URL}/admin/licencia`;
        axios({
          url,
          method: "DELETE",
        })
          .then((resp) => {
            commit("quitar");
            sessionStorage.removeItem("subscriptor");
            sessionStorage.removeItem("vencimiento");
            resolve(resp);
          })
          .catch((err) => {
            reject(err);
          });
        resolve(resolve);
      });
    },
    // auth
    login({ commit }, user) {
      return new Promise((resolve, reject) => {
        commit("auth_request");
        const url = `${process.env.VUE_APP_SERVICE_URL}/auth/Account/Login`;
        axios({
          url,
          data: user,
          method: "POST",
        })
          .then((resp) => {
            const token = `Bearer ${resp.data.token}`;
            const playload = JSON.parse(atob(token.split(".")[1]));
            const userData = playload;
            const usuario = {
              nombre: userData.unique_name,
              roles:
                userData[
                  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                ],
            };
            sessionStorage.setItem("token", token);
            sessionStorage.setItem("user_data", JSON.stringify(usuario));
            axios.defaults.headers.common.Authorization = token;

            commit("auth_success", {
              token,
              usuario,
            });
            resolve(resp);
          })
          .catch((err) => {
            commit("auth_error");
            sessionStorage.removeItem("token");
            reject(err);
          });
      });
    },
    logout({ commit }) {
      return new Promise<void>((resolve) => {
        commit("logout");
        sessionStorage.removeItem("token");
        sessionStorage.removeItem("user_data");
        delete axios.defaults.headers.common.Authorization;
        resolve();
      });
    },
  },
  getters: {
    // drawer
    drawerVisibility: (state) => state.visibility,

    // licencia
    subscriptor: (state) => state.subscriptor,
    vencimiento: (state) => state.vencimiento,
    hasLicence: (state) => state.subscriptor != null,
    licencia(state) {
      if (!state.subscriptor) {
        return null;
      }
      return { subscriptor: state.subscriptor, vencimiento: state.vencimiento };
    },
    // auth
    isLoggedIn: (state) => !!state.token,
    token: (state) => state.token,
    authStatus: (state) => state.status,
    roles: (state) => state.usuario.roles,
    usuario: (state) => state.usuario.nombre,
  },
});

import Vue from "vue";
import Vuex from "vuex";
import axios from 'axios';
import api from "@/api.js";

Vue.use(Vuex);

const auth = {
  state: {
    status: '',
    token: null,
    usuario: {
      nombre: '',
      roles: []
    },
  },
  mutations: {
    auth_request(state) {
      state.status = 'loading';
    },
    auth_success(state, payload) {
      state.status = 'success';
      state.token = payload.token;
      state.usuario = payload.usuario;
    },
    auth_error(state) {
      state.status = 'error';
    },
    logout(state) {
      state.status = '';
      state.token = null;
      state.usuario = {
        nombre: '',
        roles: []
      };
    },
  },
  actions: {
    login({
      commit
    }, user) {
      return new Promise((resolve, reject) => {
        commit('auth_request');
        let url = api.getUrl("api-account", "account/login");
        axios({
          url: url,
          data: user,
          method: 'POST'
        })
          .then(resp => {
            const token = resp.data.token;
            var playload = JSON.parse(atob(token.split('.')[1]));
            const user_data = playload;
            localStorage.setItem('token', token);
            localStorage.setItem('user_data', JSON.stringify(playload));
            axios.defaults.headers.common['Authorization'] = "Bearer " + token;
            let usuario = {
              nombre: user_data.unique_name,
              roles: user_data["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
            };
            commit('auth_success', {
              token,
              usuario
            });
            resolve(resp);
          })
          .catch(err => {
            commit('auth_error');
            localStorage.removeItem('token');
            reject(err);
          });
      });
    },
    logout({
      commit
    }) {
      return new Promise((resolve, reject) => {
        commit('logout');
        localStorage.removeItem('token');
        delete axios.defaults.headers.common['Authorization'];
        resolve();
      });
    }
  },
  getters: {
    isLoggedIn: state => {
      return !!state.token;
    },
    authStatus: state => state.status,
    roles: state => state.usuario.roles,
    usuario: state => state.usuario.nombre,
  }
};

export default auth;
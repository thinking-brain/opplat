import Vue from 'vue';
import Vuex from 'vuex';
import axios from 'axios';
import api from '@/api';

Vue.use(Vuex);

const auth = {
  state: {
    status: sessionStorage.getItem('token') ? 'success' : '',
    token: sessionStorage.getItem('token') || null,
    usuario: JSON.parse(sessionStorage.getItem('user_data')) || {
      nombre: '',
      roles: [],
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
        roles: [],
      };
    },
  },
  actions: {
    login({
      commit,
    }, user) {
      return new Promise((resolve, reject) => {
        commit('auth_request');
        const url = api.getUrl('api-account', 'account/login');
        axios({
          url,
          data: user,
          method: 'POST',
        })
          .then((resp) => {
            const token = `Bearer ${resp.data.token}`;
            const playload = JSON.parse(atob(token.split('.')[1]));
            const userData = playload;
            const usuario = {
              nombre: userData.unique_name,
              roles: userData['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
            };
            sessionStorage.setItem('token', token);
            sessionStorage.setItem('user_data', JSON.stringify(usuario));
            axios.defaults.headers.common.Authorization = token;

            commit('auth_success', {
              token,
              usuario,
            });
            resolve(resp);
          })
          .catch((err) => {
            commit('auth_error');
            sessionStorage.removeItem('token');
            reject(err);
          });
      });
    },
    logout({
      commit,
    }) {
      return new Promise((resolve) => {
        commit('logout');
        sessionStorage.removeItem('token');
        sessionStorage.removeItem('user_data');
        delete axios.defaults.headers.common.Authorization;
        resolve();
      });
    },
  },
  getters: {
    isLoggedIn: state => !!state.token,
    token: state => state.token,
    authStatus: state => state.status,
    roles: state => state.usuario.roles,
    usuario: state => state.usuario.nombre,
    userData: state => state.userData,
  },
};

export default auth;

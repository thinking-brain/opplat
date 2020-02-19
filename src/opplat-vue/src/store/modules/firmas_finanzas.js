import Vue from 'vue';
import Vuex from 'vuex';
import axios from 'axios';
import api from '@/api';

Vue.use(Vuex);

const firmasFinanzas = {
  state: {
    jefe: null,
    economico: null,
  },
  mutations: {
    quitar(state) {
      state.jefe = null;
      state.economico = null;
    },
    agregarfirmas(state, data) {
      state.jefe = data.jefe;
      state.economico = data.economico;
    },
  },
  actions: {
    cargar({
      commit,
    }) {
      return new Promise((resolve, reject) => {
        const url = api.getUrl('finanzas', 'configuraciones');
        axios({
          url,
          method: 'GET',
        })
          .then((resp) => {
            const configs = resp.data;
            const jefeNombre = configs.find(config => config.nombre === 'NombreJefe');
            const jefeCargo = configs.find(config => config.nombre === 'CargoJefe');
            const economicoNombre = configs.find(config => config.nombre === 'NombreEconomico');
            const economicoCargo = configs.find(config => config.nombre === 'CargoEconomico');
            const data = {
              jefe: { nombre: jefeNombre.valor, cargo: jefeCargo.valor },
              economico: { nombre: economicoNombre.valor, cargo: economicoCargo.valor },
            };

            commit('agregarfirmas', data);
            resolve(resp);
          })
          .catch((err) => {
            reject(err);
          });
        resolve();
      });
    },
    update({
      commit,
    }, data) {
      return new Promise((resolve, reject) => {
        if (!data) {
          reject('datos vacios');
        }
        commit('agregarfirmas', data);
        resolve(data);
      });
    },
    quitar({
      commit,
    }) {
      return new Promise((resolve, reject) => {
        const url = api.getUrl('opplat-app', 'licencia');
        // console.log(url);
        axios({
          url,
          method: 'DELETE',
        })
          .then((resp) => {
            commit('quitar');
            resolve(resp);
          })
          .catch((err) => {
            reject(err);
          });
        resolve();
      });
    },
  },
  getters: {
    jefe(state) {
      return state.jefe;
    },
    economico(state) {
      return state.economico;
    },
  },
};

export default firmasFinanzas;

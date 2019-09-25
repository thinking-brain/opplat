import Vue from 'vue';
import Vuex from 'vuex';
import axios from 'axios';
import api from '@/api';

Vue.use(Vuex);

const licencia = {
  state: {
    subscriptor: localStorage.getItem('subscriptor') || null,
    vencimiento: localStorage.getItem('vencimiento') || null,
  },
  mutations: {
    quitar(state) {
      state.subscriptor = null;
      state.vencimiento = null;
    },
    agregar(state, data) {
      state.subscriptor = data.subscriptor;
      state.vencimiento = data.vencimiento;
    },
  },
  actions: {
    agregar({
      commit,
    }, licence) {
      return new Promise((resolve) => {
        const lic = licence;
        commit('agregar', {
          subscriptor: lic.subscriptor,
          vencimiento: lic.fechaVencimiento,
        });
        localStorage.setItem('subscriptor', lic.subscriptor);
        localStorage.setItem('vencimiento', lic.fechaVencimiento);
        resolve(resolve);
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
            localStorage.removeItem('subscriptor');
            localStorage.removeItem('vencimiento');
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
    subscriptor: state => state.subscriptor,
    vencimiento: state => state.vencimiento,
    hasLicence: state => state.subscriptor != null,
    licencia(state) {
      if (!state.subscriptor) {
        return null;
      }
      return { subscriptor: state.subscriptor, vencimiento: state.vencimiento };
    },
  },
};

export default licencia;

import Vue from "vue";
import Vuex from "vuex";
import axios from 'axios';
import api from '@/api.js';

Vue.use(Vuex);

const licencia = {
  state: {
    subscriptor: null,
    vencimiento: null
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
      commit
    }, licencia) {
      return new Promise((resolve, reject) => {
        const lic = licencia;
        commit('agregar', {
          subscriptor: lic.subscriptor,
          vencimiento: lic.fechaVencimiento
        });
        resolve(resp);
      });
    },
    quitar({
      commit
    }) {
      return new Promise((resolve, reject) => {
        let url = api.getUrl("opplat-app", "licencia");
        console.log(url);
        axios({
          url: url,
          method: 'DELETE'
        })
          .then(resp => {
            commit('quitar');
            resolve(resp);
          })
          .catch(err => {
            reject(err);
          });
        resolve();
      });
    }
  },
  getters: {
    subscriptor: state => state.subscriptor,
    vencimiento: state => state.vencimiento,
    hasLicence: state => state.subscriptor != null,
    licencia: function (state) {
      if (!state.subscriptor) {
        return null;
      }
      return { subscriptor: state.subscriptor, vencimiento: state.vencimiento };
    }
  }
};

export default licencia;
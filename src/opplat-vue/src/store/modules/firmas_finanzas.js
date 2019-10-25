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
    }) {
      return new Promise((resolve, reject) => {
        const url = api.getUrl('finanzas', 'configuraciones');
        let errorCount = 0;
        //const url = api.getUrl('finanzas', 'configuraciones');
        let form = {
          Nombre: "PorcientoContingencia",
          Valor: this.porcientoContingencia
        };
        this.axios
          .put(url + '/PorcientoContingencia', form)
          .then((
            p, // console.log(p)
          ) => {
          })
          .catch((e) => {
            errorCount++;
            this.errors.push(e.response.data.errors);
          });

        form = {
          Nombre: "NombreJefe",
          Valor: this.nombreJefe
        };
        this.axios
          .put(url + '/NombreJefe', form)
          .then((
            p, // console.log(p)
          ) => {
          })
          .catch((e) => {
            errorCount++;
            this.errors.push(e.response.data.errors);
          });

        form = {
          Nombre: "CargoJefe",
          Valor: this.cargoJefe
        };
        this.axios
          .put(url + '/CargoJefe', form)
          .then((
            p, // console.log(p)
          ) => {
          })
          .catch((e) => {
            errorCount++;
            this.errors.push(e.response.data.errors);
          });
        form = {
          Nombre: "NombreEconomico",
          Valor: this.nombreEconomico
        };
        this.axios
          .put(url + '/NombreEconomico', form)
          .then((
            p, // console.log(p)
          ) => {
          })
          .catch((e) => {
            errorCount++;
            this.errors.push(e.response.data.errors);
          });
        form = {
          Nombre: "CargoEconomico",
          Valor: this.cargoEconomico
        };
        this.axios
          .put(url + '/CargoEconomico', form)
          .then((
            p, // console.log(p)
          ) => {
          })
          .catch((e) => {
            errorCount++;
            this.errors.push(e.response.data.errors);
          });
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
    quitar({
      commit,
    }) {
      return new Promise((resolve, reject) => {
        const url = api.getUrl('opplat-app', 'licencia');
        //console.log(url);
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

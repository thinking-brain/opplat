import Vue from 'vue';
import Vuex from 'vuex';

import app from './modules/app';
import drawer from './modules/drawer';
import auth from './modules/auth';
import licencia from './modules/licencia';
import getters from './getters';

Vue.use(Vuex);

export default new Vuex.Store({
  modules: {
    app,
    drawer,
    auth,
    licencia,
  },
  state: {},
  mutations: {},
  actions: {},
  getters,
});

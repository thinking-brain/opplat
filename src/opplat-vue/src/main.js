import Vue from 'vue';
import axios from 'axios';
import VueAxios from 'vue-axios';
import 'vue-snotify/styles/material.css';
import snotify, {
  SnotifyPosition,
} from 'vue-snotify';
import vuetify from './plugins/vuetify';
import router from './router/index';
import store from './store/index';
import './registerServiceWorker';
import App from './App.vue';

const options = {
  toast: {
    position: SnotifyPosition.rightBottom,
    timeout: 3000,
    showProgressBar: false,
    closeOnClick: true,
    pauseOnHover: true,
  },
};

Vue.use(snotify, options);

Vue.config.productionTip = false;
Vue.use(VueAxios, axios);
axios.defaults.headers.common['Access-Control-Allow-Origin'] = '*';
const token = sessionStorage.getItem('token');
if (token) {
  axios.defaults.headers.common.Authorization = token;
}

const vm = new Vue({
  router,
  store,
  vuetify,
  snotify,
  render: h => h(App),
}).$mount('#app');

global.vm = vm;

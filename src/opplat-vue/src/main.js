import Vue from 'vue';
import axios from 'axios';
import VueAxios from 'vue-axios';
import 'vue-snotify/styles/material.css';
import snotify, {
  SnotifyPosition,
} from 'vue-snotify';
import numeral from 'numeral';
import vuetify from './plugins/vuetify';
import router from './router/index';
import store from './store/index';
import './registerServiceWorker';
import App from './App.vue';
import notificationsHub from './notificationsHub';
import Vuelidate from 'vuelidate';
import moment from 'moment';

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
Vue.use(notificationsHub);
Vue.use(Vuelidate);
Vue.use(moment);

axios.defaults.headers.common['Access-Control-Allow-Origin'] = '*';
const token = sessionStorage.getItem('token');
if (token) {
  axios.defaults.headers.common.Authorization = token;
}

// const format = '0,0 $';
// // load a language
// numeral.language('es-Mx', {
//   delimiters: {
//     thousands: ' ',
//     decimal: ','
//   },
//   abbreviations: {
//     thousand: 'k',
//     million: 'm',
//     billion: 'b',
//     trillion: 't',
//   },
//   ordinal(number) {
//     return number === 1 ? 'er' : 's';
//   },
//   currency: {
//     symbol: '$',
//   },
// });
// numeral.language('es-Mx');

// Vue.filter('format_money', {
//   // Model => View
//   read(val) {
//     return numeral(val).format(format);
//   },
//   // View => Model
//   write(val, oldVal) {
//     const number = +val.replace(/[^\d.,]/g, '');
//     return isNaN(number) ? 0 : parseFloat(number.toFixed(2));
//   },
// });
// load a locale
numeral.register('locale', 'fr', {
  delimiters: {
    thousands: ' ',
    decimal: ',',
  },
  abbreviations: {
    thousand: 'k',
    million: 'm',
    billion: 'b',
    trillion: 't',
  },
  ordinal(number) {
    return number === 1 ? 'er' : 'ème';
  },
  currency: {
    symbol: '$',
  },
});

// switch between locales
numeral.locale('fr');
Vue.filter('format_money', value => numeral(value).format('$ 0,0.00'));
Vue.filter('format_two_decimals', value => numeral(value).format('0,0.00'));

const vm = new Vue({
  router,
  store,
  vuetify,
  snotify,
  render: h => h(App),
}).$mount('#app');

global.vm = vm;

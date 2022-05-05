import Vue from "vue";
import App from "./App.vue";
import store from "./store";
import vuetify from "./plugins/vuetify";
import router from "./router";
import axios from "axios";
import VueAxios from "vue-axios";
import numeral from "numeral";
import Vuelidate from "vuelidate";

// import './sw

Vue.filter(
  "reFormat",
  (
    value: number,
    lang: string = "ES",
    minimumFractionDigits = 2,
    maximumFractionDigits = 2,
    style: string = "currency",
    currencyDisplay: string = "symbol",
    currency: string = "CUP"
  ) => {
    return value.toLocaleString(lang, {
      style,
      currency,
      currencyDisplay,
      minimumFractionDigits,
      maximumFractionDigits,
    });
  }
);

Vue.prototype.global = store;
Vue.config.productionTip = false;
Vue.use(VueAxios, axios);
Vue.prototype.base_url = process.env.VUE_APP_MEDIA_URL;
// Vue.use(snotify, options);
// Vue.use(HighchartsVue, { tagName: "charts" });
Vue.config.productionTip = false;
Vue.use(VueAxios, axios);
// Vue.use(notificationsHub);
Vue.use(Vuelidate);
Vue.use(require("vue-moment"));
// Vue.use(Popover);

axios.defaults.headers.common["Access-Control-Allow-Origin"] = "*";
const token = sessionStorage.getItem("token");
if (token) {
  axios.defaults.headers.common.Authorization = token;
}

numeral.register("locale", "fr", {
  delimiters: {
    thousands: " ",
    decimal: ",",
  },
  abbreviations: {
    thousand: "k",
    million: "m",
    billion: "b",
    trillion: "t",
  },
  ordinal(number) {
    return number === 1 ? "er" : "ème";
  },
  currency: {
    symbol: "$",
  },
});

// switch between locales
numeral.locale("fr");
Vue.filter("format_money", (value) => numeral(value).format("$ 0,0.00"));
Vue.filter("format_two_decimals", (value) => numeral(value).format("0,0.00"));
const vueInstance = new Vue({
  router,
  store,
  vuetify,
  render: (h) => h(App),
}).$mount("#app");
export default vueInstance;

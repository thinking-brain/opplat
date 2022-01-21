import Vue from 'vue';
import '@mdi/font/css/materialdesignicons.css';
import Vuetify from 'vuetify/lib';

Vue.use(Vuetify);

const moment = require('moment')
require('moment/locale/es')
Vue.use(require('vue-moment'), {
    moment
})

export default new Vuetify({
    icons: {
        iconfont: 'mdi',
    },
});
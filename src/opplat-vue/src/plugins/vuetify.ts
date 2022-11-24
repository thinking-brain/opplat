import Vue from "vue";
import Vuetify, {
  Touch,
  VBtn,
  VIcon,
  VScrollXTransition,
  VSnackbar,
  VToolbar,
} from "vuetify/lib";
import VuetifyToast from "vuetify-toast-snackbar";
import "@mdi/font/css/materialdesignicons.css"; // Ensure you are using css-loader

Vue.use(Vuetify, {
  directives: { touch: Touch },
  components: { scrollX: VScrollXTransition, VSnackbar, VBtn, VIcon, VToolbar },
});
Vue.use(VuetifyToast, {
  classes: ["body-2"],
  y: "top",
  x: "center",
  queueable: true,
  showClose: true,
  closeIcon: "mdi-close",
  timeout: 2000,
});
const moment = require("moment");
require("moment/locale/es");
Vue.use(require("vue-moment"), {
  moment,
});
export default new Vuetify({
  icons: {
    iconfont: "mdi", // default - only for display purposes
  },
  theme: {
    options: {
      customProperties: true,
    },
    themes: {
      light: {
        primary: "#1565C0",
        secondary: "#5fb8ee",
        accent: "#efa81b",
        error: "#e83a3d",
        info: "#2196F3",
        success: "#408a40",
        warning: "#E8E800",
        neuter: "#232323",
        white: "#FFFFFF",
        gray: "#6c757d",
        metal: "#ebebeb",
      },
    },
  },
});

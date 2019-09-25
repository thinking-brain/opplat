
const drawer = {
  state: {
    status: '',
    visibility: true,
  },
  mutations: {
    show(state) {
      state.status = 'visible';
      state.visibility = true;
    },
    hide(state) {
      state.status = 'invisible';
      state.visibility = false;
    },
  },
  actions: {
    changeVisibility({
      commit,
    }, visibility) {
      return new Promise((resolve) => {
        if (visibility) {
          commit('show');
        } else {
          commit('hide');
        }
        resolve();
      });
    },
  },
  getters: {
    drawerVisibility: state => state.visibility,
  },
};

export default drawer;

const state = {
  visible: false,
  modalName: null
};

const mutations = {
  SHOW_MODAL(state, modal) {
    state.visible = true;
    state.modalName = modal;
  },
  HIDE_MODAL(state) {
    state.visible = false;
    state.modalName = null;
  }
};

const actions = {};

export default {
  namespaced: true,
  state,
  mutations,
  actions
};

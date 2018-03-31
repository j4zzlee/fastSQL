import Vue from 'vue';
const state = {
  connections: []
};

const mutations = {
  SET_CONNECTIONS(state, connections) {
    state.connections = connections;
  },
  ADD_CONNECTION(state, connection) {
    state.connections = [...state.connections, connection];
  },
  REMOVE_CONNECTION(state, id) {
    state.connections = state.connections.filter(c => {
      return c.id !== id;
    });
  }
};

const actions = {
  addConnection({ commit }, connection) {
    const connections = JSON.parse(Vue.localStorage.get('connections') || '[]') || [];
    var oldConnections = connections.filter(c => c.id !== connection.id);
    var newConnections = [...oldConnections, connection];
    Vue.localStorage.set('connections', JSON.stringify(newConnections));
    commit('SET_CONNECTIONS', newConnections);
  },
  removeConnection({commit}, id) {
    const connections = JSON.parse(Vue.localStorage.get('connections') || '[]') || [];
    Vue.localStorage.set('connections', JSON.stringify(connections.filter(c => c.id !== id)));
    commit('REMOVE_CONNECTION', id);
  },
  loadConnections({commit}) {
    const connections = JSON.parse(Vue.localStorage.get('connections') || '[]') || [];
    commit('SET_CONNECTIONS', connections);
  },
  setConnections({commit}, connections) {
    commit('SET_CONNECTIONS', connections);
  }
};

export default {
  namespaced: true,
  state,
  mutations,
  actions
};

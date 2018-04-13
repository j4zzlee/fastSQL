// import Vue from 'vue';
const state = {
  db: {
    connected: false
  }
}

const mutations = {
  SET_CONNECTED(state) {
    state.db = {
      ...state.db,
      connected: true
    }
  }
  // SET_CONNECTIONS(state, connections) {
  //   state.connections = connections;
  // },
  // ADD_CONNECTION(state, connection) {
  //   state.connections = [...state.connections, connection];
  // },
  // REMOVE_CONNECTION(state, id) {
  //   state.connections = state.connections.filter(c => {
  //     return c.id !== id;
  //   });
  // },
  // ADD_CONNECTED_CONNECTION(state, connection) {
  //   state.connectedConnections = [...state.connectedConnections, connection];
  // },
  // REMOVE_CONNECTED_CONNECTION(state, id) {
  //   const conns = state.connectedConnections.filter(c => {
  //     return c.id !== id;
  //   });
  //   state.connectedConnections = [...conns];
  // }
}

const actions = {
  // async tryConnect({commit}) {
  // }
  // addConnection({ commit }, connection) {
  //   const connections = JSON.parse(Vue.localStorage.get('connections') || '[]') || [];
  //   var oldConnections = connections.filter(c => c.id !== connection.id);
  //   var newConnections = [...oldConnections, connection];
  //   Vue.localStorage.set('connections', JSON.stringify(newConnections));
  //   commit('SET_CONNECTIONS', newConnections);
  // },
  // removeConnection({commit}, id) {
  //   const connections = JSON.parse(Vue.localStorage.get('connections') || '[]') || [];
  //   Vue.localStorage.set('connections', JSON.stringify(connections.filter(c => c.id !== id)));
  //   commit('REMOVE_CONNECTION', id);
  // },
  // loadConnections({commit}) {
  //   const connections = JSON.parse(Vue.localStorage.get('connections') || '[]') || [];
  //   commit('SET_CONNECTIONS', connections);
  // },
  // setConnections({commit}, connections) {
  //   commit('SET_CONNECTIONS', connections);
  // }
}

export default {
  namespaced: true,
  state,
  mutations,
  actions
}

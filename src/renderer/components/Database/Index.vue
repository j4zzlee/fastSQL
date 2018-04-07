<template>
  <li class="nav-item">
    <a class="nav-link" href="#" @click="onLoadTables">
      <small>
        <i :class="`fa ${isExpand ? 'fa fa-minus-square-o' : 'fa-plus-square-o'}`"></i>&nbsp;
        {{connection.profile}}
      </small>
    </a>
    <div class="btn-group btn-dropdown">
      <button type="button" class="btn btn-link" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"
        @click="onShowConnectionDropdown">
        ...
      </button>
      <div class="dropdown-menu show dropdown-menu-right" v-if="openDropdown">
        <a class="dropdown-item" href="#" @click="onNewQuery">New Query</a>
        <a class="dropdown-item" href="#">Refresh</a>
        <div class="dropdown-divider"></div>
        <a class="dropdown-item" href="#" @click="onDisconnect">Disconnect</a>
      </div>
    </div>
  </li>
</template>

<script>
// import { mapState, mapActions, mapMutations } from 'vuex';
// import uuid from 'uuid';
// import _ from 'lodash';
export default {
  name: 'database-item',
  props: {
    connection: {
      type: Object
    },
    newQuery: {
      type: Function
    },
    disconnect: {
      type: Function
    }
  },
  data() {
    return {
      openDropdown: false,
      isExpand: false
    };
  },
  async created() {
    // await this.loadConnections();
  },
  components: {
    // ConnectionModal: () => import('@/components/Connection/NewConnection'),
    // codemirror
  },
  computed: {
    // ...mapState({
    //   connections: state => state.Connection.connections,
    //   connectedConnections: state => state.Connection.connectedConnections
    // })
  },
  methods: {
    // ...mapActions('Connection', ['loadConnections']),
    // ...mapMutations('Modal', ['SHOW_MODAL']),
    // ...mapMutations('Connection', [
    //   'ADD_CONNECTED_CONNECTION',
    //   'REMOVE_CONNECTED_CONNECTION'
    // ])
    async onLoadTables () {
      const req = await this.$http.post(
        `http://localhost:5000/api/providers/${this.connection.provider.id}/tables/get`,
        this.connection.provider.options
      );
      console.log(req.data)
    },
    async onShowConnectionDropdown() {
      this.openDropdown = !this.openDropdown;
    },
    async onNewQuery() {
      this.newQuery && await this.newQuery(this.connection);
      this.openDropdown = false;
    },
    async onDisconnect() {
      this.disconnect && await this.disconnect(this.connection);
      this.openDropdown = false;
    }
  }
};
</script>
 
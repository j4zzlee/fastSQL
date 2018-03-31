<template>
  <div :class="`modal fade show`" tabindex="-1" role="dialog" :style="{display: 'block'}" @click.self="HIDE_MODAL">
    <div class="modal-dialog" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Connect</h5>
          <button type="button" class="close" data-dismiss="modal" aria-label="Close" @click="HIDE_MODAL">
            <span aria-hidden="true">&times;</span>
          </button>
        </div>
        <div class="modal-body">
          <div class="input-group mb-3">
            <div class="input-group-prepend">
              <span class="input-group-text" id="connection-profile">Profile</span>
            </div>
            
            <input type="text" class="form-control" v-model="profile" placeholder="Profile" aria-label="Profile" aria-describedby="connection-profile">
            <div :class="`input-group-btn input-group-append input-group-prepend ${isShowConnections ? 'show' : ''}`">
              <button type="button" class="btn btn-secondary dropdown-toggle"
                data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" @click="onShowConnections">
              </button>
              <div class="dropdown-menu dropdown-menu-right" v-if="isShowConnections" :style="{display: isShowConnections ? 'block': 'none'}">
                <a class="dropdown-item" href="#" v-bind:key="connection.id" v-for="connection in connections" @click="() => onChooseConnection(connection)">
                  {{connection.profile}}
                </a>
              </div>
            </div>
            <div class="input-group-append">
              <button class="btn btn-outline-secondary btn-danger" type="button" title="Delete" @click="onRemoveConnection">
                <i class="fa fa-times-circle"></i>
              </button>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-primary" @click="onConnect">Connect</button>
          <button type="button" class="btn btn-secondary" data-dismiss="modal" @click="HIDE_MODAL">Close</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { mapState, mapActions, mapMutations } from 'vuex';
import uuid from 'uuid';
export default {
  name: 'connection-modal',
  props: {},
  data() {
    return {
      id: null,
      profile: null,
      isShowConnections: false
    };
  },
  async created() {},
  mounted() {},
  components: {},
  computed: {
    ...mapState({
      connections: state => state.Connection.connections
    })
  },
  methods: {
    ...mapActions('Connection', ['addConnection', 'removeConnection']),
    ...mapMutations('Modal', ['HIDE_MODAL']),
    onChooseConnection(connection) {
      this.id = connection.id;
      this.profile = connection.profile;
      this.isShowConnections = false;
    },
    async onRemoveConnection() {
      await this.removeConnection(this.id);
      this.id = null;
      this.profile = null;
    },
    onShowConnections() {
      this.isShowConnections = !this.isShowConnections;
    },
    onConnect() {
      if (!this.profile) {
        return;
      }
      var connection = null;

      if (!this.id) {
        connection = {
          id: uuid.v4(),
          profile: this.profile
        };
        this.addConnection(connection);
      } else {
        var acceptedConns = this.connections.filter(
          c => c.profile === this.profile
        );
        if (!acceptedConns || acceptedConns.length <= 0) {
          // new
          connection = {
            id: uuid.v4(),
            profile: this.profile
          };
          this.addConnection(connection);
        } else {
          connection = acceptedConns[0];
        }
      }
      this.id = connection.id;
      this.profile = connection.profile;

      // todo: connect
      console.log('begin connect');
    }
  }
};
</script>
 
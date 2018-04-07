<template>
  <div :class="`modal fade show`" tabindex="-1" role="dialog" :style="{display: 'block'}">
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
          <div class="form-group">
            <label for="providers">Providers</label>
            <select class="form-control" v-model="currentProviderId" @change="onProviderChanged">
              <option>Choose...</option>
              <option v-for="connector in providers" v-bind:key="connector.id" v-bind:value="connector.id" >{{ connector.displayName }}</option>
            </select>
          </div>
          <div class="form-group" v-bind:key="option.Name" v-for="option in currentProvider.options">
            <label for="name">{{ option.displayName }}</label>
            <input v-if="option.type === 0" type="text" class="form-control" v-model="option.value"/>
            <textarea v-if="option.type === 1" class="form-control" v-model="option.value"/>
            <input v-if="option.type === 2" type="password" class="form-control" v-model="option.value"/>
            <div class="form-check has-success" v-if="option.type === 3">
              <label class="form-check-label">
                <input type="checkbox" class="form-check-input" v-model="option.value">
              </label>
            </div>
            <div class="file-upload" v-if="option.type === 4">
              <span class="form-control">{{option.value}}</span>
              <input class="file" type="file" @change="(e) => onFileChange(e, option)" :ref="`fileUpload${option.name}`"/>
            </div>
            <select v-if="option.type === 5" class="form-control" v-model="option.value">
              <option v-for="source in option.source" v-bind:key="source" v-bind:value="source" >{{ source }}</option>
            </select>
          </div>
          <div class="alert alert-danger" role="alert" v-if="!!errorMessage">
            {{errorMessage}}
          </div>
          <div class="alert alert-success" role="alert" v-if="!!successMessage">
            {{successMessage}}
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
import _ from 'lodash';
export default {
  name: 'connection-modal',
  props: {},
  data() {
    return {
      id: null,
      profile: null,
      isShowConnections: false,
      providers: [],
      currentProviderId: 'Choose...',
      currentProvider: { Options: [] },
      errorMessage: null,
      successMessage: null
    };
  },
  async created() {},
  async mounted() {
    this.providers = await this.getProviders();
  },
  components: {},
  computed: {
    ...mapState({
      connections: state => state.Connection.connections
    })
  },
  methods: {
    ...mapActions('Connection', ['addConnection', 'removeConnection']),
    ...mapMutations('Connection', ['ADD_CONNECTED_CONNECTION', 'REMOVE_CONNECTED_CONNECTION']),
    ...mapMutations('Modal', ['HIDE_MODAL']),
    async getProviders() {
      const req = await this.$http.get('http://localhost:5000/api/providers');
      return req.data || [];
    },
    async getProviderById(id) {
      const req = await this.$http.get(
        `http://localhost:5000/api/providers/${id}`
      );
      return req.data;
    },
    async onChooseConnection(connection) {
      this.errorMessage = null;
      this.successMessage = null;
      this.id = connection.id;
      this.profile = connection.profile;
      if (connection.provider && connection.provider.id) {
        const provider = await this.getProviderById(connection.provider.id);
        for (var i = 0; i < provider.options.length; i++) {
          const currentOption = provider.options[i];
          const connOption = _.find(connection.provider.options, {name: currentOption.name});
          if (connOption) {
            provider.options[i].value = connOption.value;
          }
        }
        this.currentProvider = {
          ...provider
        };
        this.currentProviderId = connection.provider.id
      }

      this.isShowConnections = false;
    },
    async onRemoveConnection() {
      this.errorMessage = null;
      this.successMessage = null;
      await this.removeConnection(this.id);
      this.id = null;
      this.profile = null;
      this.currentProviderId = 'Choose...';
      this.currentProvider = { Options: [] };
    },
    onShowConnections() {
      this.isShowConnections = !this.isShowConnections;
    },
    async onConnect() {
      this.errorMessage = null;
      this.successMessage = null;
      if (!this.currentProvider || !this.currentProvider.id) {
        this.errorMessage = 'Please select a provider';
        return;
      }
      // connect first
      const req = await this.$http.post(
        `http://localhost:5000/api/providers/${this.currentProviderId}/connect`,
        this.currentProvider.options
      );
      const res = req.data;
      if (!res.success) {
        this.errorMessage = res.message;
        return;
      }
      var connection = null;
      var acceptedConn = _.find(this.connections, { id: this.id });

      if (!this.id || !acceptedConn) {
        connection = {
          id: uuid.v4(),
          profile: this.profile || 'new profile',
          provider: {
            ...this.currentProvider
          }
        };
      } else {
        connection = {
          id: acceptedConn.id,
          profile: acceptedConn.profile || 'new profile',
          provider: {
            ...this.currentProvider
          }
        };
      }
      await this.addConnection(connection);
      await this.onChooseConnection(connection);
      this.successMessage = res.message;
      // push connected connection
      this.ADD_CONNECTED_CONNECTION(connection);
      this.HIDE_MODAL();
    },
    async onProviderChanged() {
      this.errorMessage = null;
      this.successMessage = null;
      const data = await this.getProviderById(this.currentProviderId);
      if (!this.id) {
        this.currentProvider = data;
        return;
      }
      const currentProfile = _.find(this.connections, { id: this.id });
      if (
        !currentProfile.provider ||
        !this.currentProvider ||
        currentProfile.provider.id !== this.currentProviderId
      ) {
        this.currentProvider = data;
        return;
      }
      this.currentProvider = currentProfile.provider;
    },
    onFileChange(e, option) {
      this.errorMessage = null;
      this.successMessage = null;
      option.Value = e.target.files[0].path;
    },
    onUpload(refName) {
      this.$refs[refName][0].click();
    }
  }
};
</script>
 
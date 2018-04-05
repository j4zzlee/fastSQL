<template>
  <div id="wrapper">
    <div id="sidenav" class="sidenav active" ref="sidenav">
      <a href="#"
        class="actions"
        @click="onConnect"
        data-toggle="tooltip"
        title="New connection">+</a>
      <ul class="nav flex-column">
        <li class="nav-item" v-for="connection in connectedConnections" v-bind:key="connection.id">
          <a class="nav-link" href="#"><small>{{connection.profile}}</small></a>
          <div class="btn-group btn-dropdown">
            <button type="button" class="btn btn-link" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"
              @click="() => onShowConnectionDropdown(connection)">
              ...
            </button>
            <div class="dropdown-menu show dropdown-menu-right" v-if="connection.id === currentConnectionDropdownId">
              <a class="dropdown-item" href="#" @click="() => onNewQuery(connection)">New Query</a>
              <a class="dropdown-item" href="#">Refresh</a>
              <div class="dropdown-divider"></div>
              <a class="dropdown-item" href="#" @click="() => onDisconnect(connection.id)">Disconnect</a>
            </div>
          </div>
        </li>
      </ul>
    </div>
    <main class="sql-container" v-if="tabs && tabs.length">
      <ul class="nav nav-tabs">
        <li class="nav-item" v-bind:key="tab.id" v-for="tab in tabs" @click="() => onTabSelect(tab)">
          <a :class="`nav-link ${selectedTabId === tab.id ? 'active' : ''}`" href="#">{{tab.name}}</a>
        </li>
      </ul>
      <div class="sql-actions">
        <button type="button" class="btn btn-link" @click.prevent="onExecuteScript">
          <i class="fa fa-play"></i>
        </button>
        <button type="button" class="btn btn-link">
          <i class="fa fa-floppy-o"></i>
        </button>
        <button type="button" class="btn btn-link">
          <i class="fa fa-trash-o"></i>
        </button>
      </div>
      <div class="codemirror">
        <codemirror v-model="code" :options="cmOptions" @input="onCodeChange"></codemirror> 
      </div>
      <div class="sql-result" v-if="(queryResults && queryResults.length > 0) || errorMessage">
        <div class="alert alert-danger" role="alert" v-if="!!errorMessage">
          {{errorMessage}}
        </div>
        <div class="form-group" v-for="r in queryResults" v-bind:key="r.Id">
          <span class="form-control" v-if="r.RecordsAffected > -1">Records affected: {{r.RecordsAffected}}</span>
          <table class="table table-striped" v-if="r.Rows && r.Rows.length > 0">
            <thead>
              <tr>
                <th scope="col" v-bind:key="key" v-for="key in Object.keys(r.Rows[0])">{{key}}</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="row in r.Rows" v-bind:key="row.Id">
                <td scope="row" v-bind:key="key" v-for="key in Object.keys(r.Rows[0])">{{row[key]}}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </main>
  </div>
</template>

<script>
import { mapState, mapActions, mapMutations } from 'vuex';
import { codemirror } from 'vue-codemirror';
import 'codemirror/lib/codemirror.css';
import 'codemirror/mode/sql/sql';
import uuid from 'uuid';
import _ from 'lodash';
export default {
  name: 'home-page',
  data() {
    return {
      isOpenNav: false,
      currentConnectionDropdownId: '',
      selectedTabId: '',
      tabs: [],
      queryResults: null,
      errorMessage: null,
      code: '',
      cmOptions: {
        tabSize: 4,
        lineNumbers: true,
        line: true,
        mode: 'text/x-sql'
      }
    };
  },
  async created() {
    await this.loadConnections();
  },
  components: {
    ConnectionModal: () => import('@/components/Connection/NewConnection'),
    codemirror
  },
  computed: {
    ...mapState({
      connections: state => state.Connection.connections,
      connectedConnections: state => state.Connection.connectedConnections
    })
  },
  methods: {
    ...mapActions('Connection', ['loadConnections']),
    ...mapMutations('Modal', ['SHOW_MODAL']),
    ...mapMutations('Connection', [
      'ADD_CONNECTED_CONNECTION',
      'REMOVE_CONNECTED_CONNECTION'
    ]),
    async onConnect() {
      this.SHOW_MODAL('NewConnection');
    },
    async onShowConnectionDropdown(connection) {
      if (this.currentConnectionDropdownId) {
        if (this.currentConnectionDropdownId === connection.id) {
          this.currentConnectionDropdownId = null;
        } else {
          this.currentConnectionDropdownId = connection.id;
        }
      } else {
        this.currentConnectionDropdownId = connection.id;
      }
    },
    async onDisconnect(connectionId) {
      this.REMOVE_CONNECTED_CONNECTION(connectionId);
    },
    async onTabSelect(tab) {
      this.selectedTabId = tab.id;
      this.code = tab.sql;
    },
    async onNewQuery(connection) {
      const len = this.tabs.length + 1;
      const selectedTabId = uuid.v4();
      const tabName = `${connection.profile} - sql ${len}`;
      this.tabs.push({
        id: selectedTabId,
        name: tabName,
        connectionId: connection.id,
        sql: ''
      });
      this.selectedTabId = selectedTabId;
      this.currentConnectionDropdownId = null;
    },
    async onCodeChange(newVal) {
      for (var i = 0; i < this.tabs.length; i++) {
        if (this.tabs[i].id === this.selectedTabId) {
          this.tabs[i].sql = newVal;
        }
      }
    },
    async onExecuteScript() {
      this.errorMessage = null;
      this.queryResults = null;
      const tab = _.find(this.tabs, { id: this.selectedTabId });
      const connection = _.find(this.connectedConnections, {
        id: tab.connectionId
      });
      const providerId = connection.provider.Id;
      const req = await this.$http.post(
        `http://localhost:5000/api/providers/${providerId}/query`,
        {
          RawQuery: tab.sql,
          Options: connection.provider.Options
        }
      );
      const res = req.data;
      if (!res.success) {
        this.errorMessage = res.message;
      }
      this.queryResults = res.data;
    }
  }
};
</script>
 
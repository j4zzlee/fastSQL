<template>
  <main>
    <br>
    <button type="button" class="btn btn-primary btn-lg btn-block" @click="onNewConnection">Add a connection</button>
    <br>
    <div v-for="con in connections" v-bind:key="con.Id">
      <div class="card">
        <div class="card-header">
          {{con.Name}} <span class="text-muted small">({{con.Id}})</span>
          <a class="btn btn-link btn-sm pull-right" @click="() => onDelete(con)"><i class="fa fa-trash-o"></i></a>
          <a class="btn btn-link btn-sm pull-right" @click="() => onEdit(con)"><i class="fa fa-pencil-square-o"></i></a>
          <a class="btn btn-link btn-sm pull-right" @click="() => onTryConnect(con)"><i class="fa fa-play"></i></a>
        </div>
        <div class="card-body">
          <div :class="`alert alert-${messages.id === con.Id && messages.success ? 'success' : 'danger'}`" v-if="messages && messages.id === con.Id">{{messages.message}}</div>
          <div class="form-group row" v-for="key in ['Name', 'Description']" v-bind:key="key">
            <label :for="`${con.Id}_${key}`" class="col-sm-2 col-form-label col-form-lable-right">{{key}}</label>
            <div class="col-sm-10">
              <input type="text" readonly class="form-control-plaintext" :id="`${con.Id}_${key}`" :value="con[key]">
            </div>
          </div>
        </div>
      </div>
      <br>
    </div>
    
    <NewConnection :title="connectionModalTitle" 
      :visible="openNewConnection" 
      :onSave="onSave" 
      :onClose="onClose"
      :connection="currentConnection">
    </NewConnection>
  </main>
</template>

<script>
export default {
  name: 'global-connections-page',
  data() {
    return {
      connectionModalTitle: 'New Connection',
      connections: [],
      messages: {},
      openNewConnection: false,
      currentConnection: null
    }
  },
  async created() {
    this.connections = await this.getConnections()
  },
  components: {
    Dropdown: () => import('@/components/Controls/Dropdown'),
    NewConnection: () => import('@/components/Connections/NewConnection')
  },
  computed: {},
  methods: {
    async onDelete(conn) {
      if (confirm('Are you sure you want to delete?')) {
        await this.$http.delete(
          `${process.env.BACKEND}/api/connections/${conn.Id}`
        )
        this.connections = await this.getConnections()
      }
    },
    async onEdit(con) {
      this.openNewConnection = true
      this.connectionModalTitle = 'Edit Connection'
      this.currentConnection = con
    },
    async onNewConnection() {
      this.openNewConnection = true
      this.connectionModalTitle = 'New Connection'
      this.currentConnection = null
    },
    async getConnections() {
      var res = await this.$http.get(`${process.env.BACKEND}/api/connections`)
      return res.data
    },
    async onSave() {
      this.connections = await this.getConnections()
      this.openNewConnection = false
    },
    async onClose() {
      this.openNewConnection = false
    },
    async onTryConnect(connection) {
      this.messages = {}
      const res = await this.$http.post(
        `${process.env.BACKEND}/api/providers/${connection.ProviderId}/connect`,
        connection.Options
      )
      this.messages = {
        id: connection.Id,
        success: res.data.success,
        message: res.data.message
      }
    }
  }
}
</script>
 
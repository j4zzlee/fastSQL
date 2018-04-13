<template>
<Modal :title="title" :isShow="visible" :canClose="true" :onHide="onClose">
  <div slot="body">
    <div class="form-group">
      <label for="connection_name">Connection Name</label>
      <input type="text" class="form-control" id="connection_name" v-model="connectionName">
    </div>
    <div class="form-group">
      <label for="connection_description">Connection Description</label>
      <textarea rows="4" class="form-control" id="connection_description" v-model="connectionDescription"/>
    </div>
    <div class="form-group">
      <label for="providers">Providers</label>
      <select class="form-control" v-model="currentProviderId" @change="onProviderChanged">
        <option>Choose...</option>
        <option v-for="connector in providers" v-bind:key="connector.id" v-bind:value="connector.id" >{{ connector.displayName }}</option>
      </select>
    </div>
    <div class="alert alert-danger" v-if="errorMessage">{{errorMessage}}</div>
    <div class="alert alert-success" v-if="successMessage">{{successMessage}}</div>
    <DynamicOption v-bind:key="option.Name"
      v-for="option in currentProvider.options"
      :option="option"
      @change="(val) => option.value = val"></DynamicOption>
  </div>
  <div slot="footer">
    <button type="button" class="btn btn-primary" @click="onConnect">
      <span class="fa fa-spinner spin" v-if="loading"></span>
      Connect
    </button>
    <button type="button" class="btn btn-primary" @click="onSaveHandler">Save</button>
    <button type="button" class="btn btn-primary" @click="onClose">Close</button>
  </div>
</Modal>
</template>
<script>
export default {
  name: 'connection-modal',
  props: {
    title: String,
    visible: Boolean,
    onClose: Function,
    onSave: Function
  },
  data() {
    return {
      connectionName: null,
      connectionDescription: null,
      loading: false,
      errorMessage: null,
      successMessage: null,
      providers: [],
      currentProvider: {},
      currentProviderId: null
    }
  },
  async created() {},
  async mounted() {
    this.providers = await this.getProviders()
  },
  components: {
    Modal: () => import('@/components/Controls/Modal'),
    DynamicOption: () => import('@/components/Controls/DynamicOption')
  },
  computed: {},
  methods: {
    async onSaveHandler() {
      const res = await this.$http.post(
        `${process.env.BACKEND}/api/connections`,
        {
          name: this.connectionName,
          description: this.connectionDescription,
          providerId: this.currentProvider.id,
          options: this.currentProvider.options
        }
      )
      this.onSave && this.onSave(res.data)
      this.onClose && this.onClose()
    },
    async onConnect() {
      try {
        this.loading = true
        this.errorMessage = null
        this.successMessage = null
        const res = await this.$http.post(
          `${process.env.BACKEND}/api/providers/${
            this.currentProvider.id
          }/connect`,
          this.currentProvider.options
        )
        if (res.data.success) {
          this.successMessage = 'Connected Successfully.'
        } else {
          this.errorMessage = res.data.message
        }
      } catch (err) {
        this.errorMessage = err.message
      } finally {
        this.loading = false
      }
    },
    async getProviders() {
      const req = await this.$http.get(`${process.env.BACKEND}/api/providers`)
      return req.data || []
    },
    async getProviderById(id) {
      const req = await this.$http.get(
        `${process.env.BACKEND}/api/providers/${id}`
      )
      return req.data
    },
    async onProviderChanged() {
      this.errorMessage = null
      this.successMessage = null
      this.currentProvider = await this.getProviderById(this.currentProviderId)
    }
  }
}
</script>
 
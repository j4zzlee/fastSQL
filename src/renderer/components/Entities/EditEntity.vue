<template>
<main>
  <div class="form-group">
    <label for="entity-name" class="font-weight-bold font-italic">Entity Name</label>
    <input type="text" class="form-control" id="entity-name" v-model="entityName">
  </div>
  <div class="form-group">
    <label for="entity-description" class="font-weight-bold font-italic">Entity Description</label>
    <textarea rows="4" class="form-control" id="entity-description" v-model="entityDescription"/>
  </div>
  <div class="form-group">
    <div class="row">
      <label class="font-weight-bold font-italic col-form-label col-sm-2 pt-0" for="entity-enabled">Enabled</label>
      <div class="col-sm-10">
        <div class="form-check">
          <input type="checkbox" v-model="entityEnabled" class="form-check-input" id="entity-enabled">
        </div>
      </div>
    </div>
  </div>
  <div class="form-group">
    <label for="source-processor" class="font-weight-bold font-italic">Source Processor</label>
    <select class="form-control" v-model="sourceProcessorId" id="source-processor" @change="onSourceProcessorChanged">
      <option v-for="processor in processors" v-bind:key="processor.id" v-bind:value="processor.id">{{ processor.name }}</option>
    </select>
  </div>
  <div class="form-group">
    <label for="source-connection" class="font-weight-bold font-italic">Source Connection</label>
    <select class="form-control" v-model="sourceConnectionId" id="source-connection" @change="onSourceConnectionChanged">
      <option v-for="connection in connections" v-bind:key="connection.id" v-bind:value="connection.id">{{ connection.name }}</option>
    </select>
  </div>
  <div class="form-group">
    <label for="destination-processor" class="font-weight-bold font-italic">Destination Processor</label>
    <select class="form-control" v-model="destinationProcessorId" id="destination-processor" @change="onDestinationProcessorChanged">
      <option v-for="processor in processors" v-bind:key="processor.id" v-bind:value="processor.id">{{ processor.name }}</option>
    </select>
  </div>
  <div class="form-group">
    <label for="destination-connection" class="font-weight-bold font-italic">Destination Connection</label>
    <select class="form-control" v-model="destinationConnectionId" id="destination-connection" @change="onDestinationConnectionChanged">
      <option v-for="connection in connections" v-bind:key="connection.id" v-bind:value="connection.id">{{ connection.name }}</option>
    </select>
  </div>
  <div class="card" v-if="pullerOptions && pullerOptions.length">
    <div class="card-header">
      <span class="font-weight-bold">Puller Options</span>
      <a class="btn btn-link btn-sm pull-right" title="Preview" @click="onTryPull"><i class="fa fa-play"></i></a>
    </div>
    <div class="card-body">
      <DynamicOption v-bind:key="option.name"
        v-for="option in pullerOptions"
        :option="option"
        @change="(val) => option.value = val"></DynamicOption>
    </div>
  </div>
  <div class="card" v-if="indexerOptions && indexerOptions.length">
    <div class="card-header">
      <span class="font-weight-bold">Indexer Options</span>
    </div>
    <div class="card-body">
      <DynamicOption v-bind:key="option.name"
        v-for="option in indexerOptions"
        :option="option"
        @change="(val) => option.value = val"></DynamicOption>
    </div>
  </div>
  <div class="card" v-if="pusherOptions && pusherOptions.length">
    <div class="card-header">
      <span class="font-weight-bold">Pusher Options</span>
    </div>
    <div class="card-body">
      <DynamicOption v-bind:key="option.name"
        v-for="option in pusherOptions"
        :option="option"
        @change="(val) => option.value = val"></DynamicOption>
    </div>
  </div>
  <br>
  <div class="form-group">
    <button type="button" class="btn btn-primary" @click="onSaveHandler">Save</button>
    <button type="button" class="btn btn-default" @click="onManageEntity">Manage</button>
  </div>
  <PreviewDataModal :isShow="previewVisible" :data="previewData" :onClose="onClosePreview"></PreviewDataModal>
</main>
</template>
<script>
export default {
  name: 'edit-entity-page',
  props: {},
  data() {
    return {
      entityName: '',
      entityDescription: '',
      entityEnabled: true,
      sourceConnectionId: '',
      sourceConnection: {},
      destinationConnectionId: '',
      destinationConnection: {},
      sourceProcessorId: '',
      sourceProcessor: {},
      destinationProcessorId: '',
      destinationProcessor: {},
      processors: [],
      connections: [],
      pullerOptions: [],
      indexerOptions: [],
      pusherOptions: [],
      entityOptions: [],
      previewVisible: false,
      previewData: []
    }
  },
  async created() {},
  async mounted() {
    this.connections = await this.getConnections()
    this.processors = await this.getProcessors()
    if (this.entityId) {
      var res = await this.$http.get(
        `${process.env.BACKEND}/api/entities/${this.entityId}`
      )
      this.entityName = res.data.name
      this.entityDescription = res.data.description
      this.entityEnabled = (res.data.state & 1) === 0
      this.sourceConnectionId = res.data.sourceConnectionId
      this.sourceConnection = this.connections.filter(
        c => c.id === this.sourceConnectionId
      )[0]
      this.sourceProcessorId = res.data.sourceProcessorId
      this.sourceProcessor = this.processors.filter(
        p => p.id === this.sourceProcessorId
      )[0]
      this.destinationConnectionId = res.data.destinationConnectionId
      this.destinationConnection = this.connections.filter(
        c => c.id === this.destinationConnectionId
      )[0]
      this.destinationProcessorId = res.data.destinationProcessorId
      this.destinationProcessor = this.processors.filter(
        p => p.id === this.destinationProcessorId
      )[0]
      this.entityOptions = res.data.options || []
      await this.getOptions()
    }
  },
  watch: {},
  components: {
    Modal: () => import('@/components/Controls/Modal'),
    DynamicOption: () => import('@/components/Controls/DynamicOption'),
    PreviewDataModal: () => import('@/components/Controls/PreviewDataModal')
  },
  computed: {
    entityId() {
      return this.$route.params.id
    }
  },
  methods: {
    async getConnections() {
      var res = await this.$http.get(`${process.env.BACKEND}/api/connections`)
      return res.data
    },
    async getProcessors() {
      var res = await this.$http.get(`${process.env.BACKEND}/api/processors`)
      return res.data
    },
    async getOptions() {
      var res = await this.$http.post(
        `${process.env.BACKEND}/api/entities/options/template`,
        {
          sourceConnectionId: this.sourceConnectionId,
          destinationConnectionId: this.destinationConnectionId,
          sourceProcessorId: this.sourceProcessorId,
          destinationProcessorId: this.destinationProcessorId
        }
      )
      this.pullerOptions = res.data.puller
      for (var i = 0; i < this.pullerOptions.length; i++) {
        var pullerOptions = this.entityOptions.filter(
          e => e.name === this.pullerOptions[i].name
        )
        var pullerOption =
          pullerOptions && pullerOptions.length ? pullerOptions[0] : null
        if (pullerOption) {
          this.pullerOptions[i].value = pullerOption.value
        }
      }
      this.pusherOptions = res.data.pusher
      for (var k = 0; k < this.pusherOptions.length; k++) {
        var pusherOptions = this.entityOptions.filter(
          e => e.name === this.pusherOptions[k].name
        )
        var pusherOption =
          pusherOptions && pusherOptions.length ? pusherOptions[0] : null
        if (pusherOption) {
          this.pusherOptions[k].value = pusherOption.value
        }
      }
      this.indexerOptions = res.data.indexer
      for (var j = 0; j < this.indexerOptions.length; j++) {
        var indexerOptions = this.entityOptions.filter(
          e => e.name === this.indexerOptions[j].name
        )
        var indexerOption =
          indexerOptions && indexerOptions.length ? indexerOptions[0] : null
        if (indexerOption) {
          this.indexerOptions[j].value = indexerOption.value
        }
      }
    },
    async onSourceProcessorChanged() {
      this.sourceProcessor = {
        ...this.processors.filter(p => p.id === this.sourceProcessorId)[0]
      }
      await this.getOptions()
    },
    async onDestinationProcessorChanged() {
      this.destinationProcessor = {
        ...this.processors.filter(p => p.id === this.destinationProcessorId)[0]
      }
      await this.getOptions()
    },
    async onSourceConnectionChanged() {
      this.sourceConnection = {
        ...this.connections.filter(p => p.id === this.sourceConnectionId)[0]
      }
      await this.getOptions()
    },
    async onDestinationConnectionChanged() {
      this.destinationConnection = {
        ...this.connections.filter(
          p => p.id === this.destinationConnectionId
        )[0]
      }
      await this.getOptions()
    },
    async onSaveHandler() {
      let params = {
        name: this.entityName,
        description: this.entityDescription,
        sourceProcessorId: this.sourceProcessorId,
        destinationProcessorId: this.destinationProcessorId,
        sourceConnectionId: this.sourceConnectionId,
        destinationConnectionId: this.destinationConnectionId,
        enabled: this.entityEnabled,
        options: [
          ...this.pullerOptions,
          ...this.indexerOptions,
          ...this.pusherOptions
        ]
      }
      if (!this.entityId) {
        await this.$http.post(`${process.env.BACKEND}/api/entities`, params)
      } else {
        await this.$http.put(
          `${process.env.BACKEND}/api/entities/${this.entityId}`,
          params
        )
      }
      await this.$router.push({ name: 'entities' })
    },
    async onManageEntity() {
      await this.$router.push({
        name: 'manage-entity',
        params: { id: this.entityId }
      })
    },
    async onTryPull() {
      const res = await this.$http.post(
        `${process.env.BACKEND}/api/pullers/entity/${this.entityId}`, {}
      )
      this.previewData = res.data.data
      this.previewVisible = true
    },
    onClosePreview () {
      this.previewVisible = false
    }
  }
}
</script>
 
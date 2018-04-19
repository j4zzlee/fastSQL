<template>
<main>
  <div class="form-group">
    <label for="attribute-name" class="font-weight-bold font-italic">Attribute Name</label>
    <input type="text" class="form-control" id="attribute-name" v-model="attributeName">
  </div>
  <div class="form-group">
    <label for="attribute-description" class="font-weight-bold font-italic">Attribute Description</label>
    <textarea rows="4" class="form-control" id="attribute-description" v-model="attributeDescription"/>
  </div>
  <div class="form-group">
    <div class="row">
      <label class="font-weight-bold font-italic col-form-label col-sm-2 pt-0" for="attribute-enabled">Enabled</label>
      <div class="col-sm-10">
        <div class="form-check">
          <input type="checkbox" class="form-check-input" v-model="attributeEnabled" id="attribute-enabled">
        </div>
      </div>
    </div>
  </div>
  <div class="form-group">
    <label for="entity" class="font-weight-bold font-italic">Entity</label>
    <select class="form-control" v-model="entityId" id="entity" @change="onEntityChanged">
      <option v-for="entity in entities" v-bind:key="entity.id" v-bind:value="entity.id">{{ entity.name }}</option>
    </select>
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
      <a class="btn btn-link btn-sm pull-right" title="Preview"><i class="fa fa-play"></i></a>
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
  </div>
</main>
</template>
<script>
export default {
  name: 'edit-attribute-page',
  props: {},
  data() {
    return {
      attributeName: '',
      attributeDescription: '',
      attributeEnabled: true,
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
      entities: [],
      entityId: '',
      entity: {},
      attributeOptions: [],
      pullerOptions: [],
      pusherOptions: [],
      indexerOptions: []
    }
  },
  async created() {},
  async mounted() {
    this.connections = await this.getConnections()
    this.processors = await this.getProcessors()
    this.entities = await this.getEntities()
    if (this.attributeId) {
      var res = await this.$http.get(`${process.env.BACKEND}/api/attributes/${this.attributeId}`)
      this.attributeName = res.data.name
      this.attributeDescription = res.data.description
      this.attributeEnabled = (res.data.state & 1) === 0
      this.entityId = res.data.entityId
      this.entity = this.entities.filter(e => e.id === this.entityId)
      this.sourceConnectionId = res.data.sourceConnectionId
      this.sourceConnection = this.connections.filter(c => c.id === this.sourceConnectionId)[0]
      this.sourceProcessorId = res.data.sourceProcessorId
      this.sourceProcessor = this.processors.filter(p => p.id === this.sourceProcessorId)[0]
      this.destinationConnectionId = res.data.destinationConnectionId
      this.destinationConnection = this.connections.filter(c => c.id === this.destinationConnectionId)[0]
      this.destinationProcessorId = res.data.destinationProcessorId
      this.destinationProcessor = this.processors.filter(p => p.id === this.destinationProcessorId)[0]
      this.attributeOptions = res.data.options || []
      await this.getOptions()
    }
  },
  watch: {},
  components: {
    Modal: () => import('@/components/Controls/Modal'),
    DynamicOption: () => import('@/components/Controls/DynamicOption')
  },
  computed: {
    attributeId() {
      return this.$route.params.id
    }
  },
  methods: {
    async getConnections() {
      var res = await this.$http.get(`${process.env.BACKEND}/api/connections`)
      return res.data
    },
    async getProcessors() {
      var res = await this.$http.get(`${process.env.BACKEND}/api/processors?type=2`)
      return res.data
    },
    async getEntities() {
      var res = await this.$http.get(`${process.env.BACKEND}/api/entities`)
      return res.data
    },
    async getOptions() {
      var res = await this.$http.post(
        `${process.env.BACKEND}/api/attributes/options/template`,
        {
          entityId: this.entityId,
          sourceConnectionId: this.sourceConnectionId,
          destinationConnectionId: this.destinationConnectionId,
          sourceProcessorId: this.sourceProcessorId,
          destinationProcessorId: this.destinationProcessorId
        }
      )
      this.pullerOptions = res.data.puller
      for (var i = 0; i < this.pullerOptions.length; i++) {
        var pullerOptions = this.attributeOptions.filter(e => e.name === this.pullerOptions[i].name);
        var pullerOption = pullerOptions && pullerOptions.length ? pullerOptions[0] : null;
        if (pullerOption) {
          this.pullerOptions[i].value = pullerOption.value
        }
      }
      this.pusherOptions = res.data.pusher
      for (var k = 0; k < this.pusherOptions.length; k++) {
        var pusherOptions = this.attributeOptions.filter(e => e.name === this.pusherOptions[k].name);
        var pusherOption = pusherOptions && pusherOptions.length ? pusherOptions[0] : null;
        if (pusherOption) {
          this.pusherOptions[k].value = pusherOption.value
        }
      }
      this.indexerOptions = res.data.indexer
      for (var j = 0; j < this.indexerOptions.length; j++) {
        var indexerOptions = this.attributeOptions.filter(e => e.name === this.indexerOptions[j].name);
        var indexerOption = indexerOptions && indexerOptions.length ? indexerOptions[0] : null;
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
    async onEntityChanged() {
      this.entity = {
        ...this.entities.filter(p => p.id === this.entityId)[0]
      }
      await this.getOptions()
    },
    async onSaveHandler() {
      let params = {
        name: this.attributeName,
        description: this.attributeDescription,
        entityId: this.entityId,
        sourceProcessorId: this.sourceProcessorId,
        destinationProcessorId: this.destinationProcessorId,
        sourceConnectionId: this.sourceConnectionId,
        destinationConnectionId: this.destinationConnectionId,
        enabled: this.attributeEnabled,
        options: [
          ...this.pullerOptions,
          ...this.indexerOptions,
          ...this.pusherOptions
        ]
      }
      if (!this.attributeId) {
        await this.$http.post(
          `${process.env.BACKEND}/api/attributes`,
          params
        )
      } else {
        await this.$http.put(
          `${process.env.BACKEND}/api/attributes/${this.attributeId}`,
          params
        )
      }
    }
  }
}
</script>
 
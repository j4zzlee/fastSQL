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
    <label for="processor" class="font-weight-bold font-italic">Processor</label>
    <select class="form-control" v-model="processorId" id="processor">
      <option v-for="processor in processors" v-bind:key="processor.id" v-bind:value="processor.id" @click="() => onProcessorChanged(processor)">{{ processor.name }}</option>
    </select>
  </div>
  <div class="form-group">
    <label for="source-connection" class="font-weight-bold font-italic">Source Connection</label>
    <select class="form-control" v-model="sourceConnectionId" id="source-connection">
      <option v-for="connection in connections" v-bind:key="connection.id" v-bind:value="connection.id" @click="() => onSourceConnectionChanged(connection)">{{ connection.name }}</option>
    </select>
  </div>
  <div class="form-group">
    <label for="destination-connection" class="font-weight-bold font-italic">Destination Connection</label>
    <select class="form-control" v-model="destinationConnectionId" id="destination-connection">
      <option v-for="connection in connections" v-bind:key="connection.id" v-bind:value="connection.id" @click="() => onDestinationConnectionChanged(connection)">{{ connection.name }}</option>
    </select>
  </div>
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
      sourceConnectionId: '',
      sourceConnection: {},
      destinationConnectionId: '',
      destinationConnection: {},
      processorId: '',
      processor: {},
      processors: [],
      connections: []
    }
  },
  async created() {},
  async mounted() {
    this.connections = await this.getConnections()

    // processors should always have values
    this.processors = await this.getProcessors()
    this.processor = { ...this.processors[0] }
    this.processorId = this.processor.id
  },
  watch: {},
  components: {
    Modal: () => import('@/components/Controls/Modal'),
    DynamicOption: () => import('@/components/Controls/DynamicOption')
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
    onProcessorChanged(processor) {
      this.processorId = processor.id
      this.processor = { ...processor }
    },
    onSourceConnectionChanged(connection) {
      this.sourceConnectionId = connection.id
      this.sourceConnection = {...connection}
    },
    onDestinationConnectionChanged(connection) {
      this.destinationConnectionId = connection.id
      this.destinationConnection = {...connection}
    }
  }
}
</script>
 
<template>
  <main>
    <div class="card">
      <div class="card-header">
        <span class="font-weight-bold">Entity Information</span>
        <a class="btn btn-link btn-sm pull-right" title="Force change"><i class="fa fa-chain-broken"></i></a>
        <a class="btn btn-link btn-sm pull-right" title="Edit" @click="onEdit"><i class="fa fa-pencil-square-o"></i></a>
        <a class="btn btn-link btn-sm pull-right" title="Pull"><i class="fa fa-play"></i></a>
        <a class="btn btn-link btn-sm pull-right" title="Initialize"><i class="fa fa-university"></i></a>
      </div>
      <div class="card-body">
        <div class="form-group">
          <label for="entity-name" class="font-weight-bold font-italic">Name</label>
          <span class="form-control-plaintext" id="entity-name">{{entity.name}}</span>
        </div>
        <div class="form-group">
          <label for="entity-description" class="font-weight-bold font-italic">Description</label>
          <span class="form-control-plaintext" id="entity-description">{{entity.description}}</span>
        </div>
      </div>
    </div>
  </main>
</template>

<script>
export default {
  name: 'manage-entity-page',
  data() {
    return {
      entity: {}
    }
  },
  async mounted() {
    var res = await this.$http.get(`${process.env.BACKEND}/api/entities/${this.entityId}`)
    this.entity = res.data
    // this.entities = await this.getEntities()
  },
  components: {
    Dropdown: () => import('@/components/Controls/Dropdown')
  },
  computed: {
    entityId() {
      return this.$route.params.id
    }
  },
  methods: {
    onInit() {},
    onEdit() {
      this.$router.push({name: 'edit-entity', params: {id: this.entityId}})
    }
  }
}
</script>
 
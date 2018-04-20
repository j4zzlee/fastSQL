<template>
  <main>
    <div class="row p-1 justify-content-md-center">
      <div class="col-md-auto">
        <button type="button" class="btn btn-primary btn-lg btn-block" @click="onNewEntity">Add an entity</button>
      </div>
    </div>
    <div class="card" v-if="entities && entities.length" v-for="entity in entities" v-bind:key="entity.id">
      <div class="card-header">
        <span class="font-weight-bold">{{entity.name}}
          <small class="text-muted">({{entity.id}})</small>
          <a class="btn btn-link btn-sm pull-right" @click="() => onDeleteEntity(entity)"><i class="fa fa-trash-o"></i></a>
          <a class="btn btn-link btn-sm pull-right" @click="() => onEditEntity(entity)"><i class="fa fa-pencil-square-o"></i></a>
          <a class="btn btn-link btn-sm pull-right" @click="() => onManageEntity(entity)"><i class="fa fa-play"></i></a>
        </span>
      </div>
      <div class="card-body">
        <div class="form-group">
          <label for="entity-description" class="font-weight-bold font-italic">Description</label>
          <span rows="4" class="form-control-plaintext" id="entity-description"> {{entity.description}} </span>
        </div>
      </div>
    </div>
  </main>
</template>

<script>
export default {
  name: 'entities-page',
  data() {
    return {
      entities: []
    }
  },
  async mounted() {
    this.entities = await this.getEntities()
  },
  components: {
    Dropdown: () => import('@/components/Controls/Dropdown')
  },
  computed: {},
  methods: {
    onNewEntity() {
      this.$router.push({ name: 'edit-entity' }) /* , params: { id: '123' } */
    },
    async getEntities() {
      var res = await this.$http.get(`${process.env.BACKEND}/api/entities`)
      return res.data
    },
    async onDeleteEntity(entity) {
      await this.$http.delete(`${process.env.BACKEND}/api/entities/${entity.id}`)
      this.entities = await this.getEntities()
    },
    onEditEntity(entity) {
      this.$router.push({name: 'edit-entity', params: {id: entity.id}})
    },
    async onManageEntity(entity) {
      await this.$router.push({name: 'manage-entity', params: {id: entity.id}})
    }
  }
}
</script>
 
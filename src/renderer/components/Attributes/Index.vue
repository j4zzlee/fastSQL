<template>
  <main>
    <div class="row p-1 justify-content-md-center">
      <div class="col-md-auto">
        <button type="button" class="btn btn-primary btn-lg btn-block" @click="onNewAttribute">Add an attribute</button>
      </div>
    </div>
    <div class="card" v-if="attributes && attributes.length" v-for="attribute in attributes" v-bind:key="attribute.id">
      <div class="card-header">
        <span class="font-weight-bold">{{attribute.name}}
          <small class="text-muted">({{attribute.id}})</small>
          <a class="btn btn-link btn-sm pull-right" @click="() => onDeleteAttribute(attribute)"><i class="fa fa-trash-o"></i></a>
          <a class="btn btn-link btn-sm pull-right" @click="() => onEditAttribute(attribute)"><i class="fa fa-pencil-square-o"></i></a>
          <a class="btn btn-link btn-sm pull-right" @click="() => onManageAttribute(attribute)"><i class="fa fa-play"></i></a>
        </span>
      </div>
      <div class="card-body">
        <div class="form-group">
          <label for="attrubte-description" class="font-weight-bold font-italic">Description</label>
          <span rows="4" class="form-control-plaintext" id="attribute-description"> {{attribute.description}} </span>
        </div>
      </div>
    </div>
  </main>
</template>

<script>

export default {
  name: 'attributes-page',
  data() {
    return {
      attributes: []
    };
  },
  async mounted() {
    this.attributes = await this.getAttributes()
  },
  components: {
    Dropdown: () => import('@/components/Controls/Dropdown')
  },
  computed: {
  },
  methods: {
    onNewAttribute () {
      this.$router.push({ name: 'edit-attribute' })
    },
    async getAttributes() {
      var res = await this.$http.get(`${process.env.BACKEND}/api/attributes`)
      return res.data
    },
    async onDeleteAttribute(a) {
      await this.$http.delete(`${process.env.BACKEND}/api/entities/${a.id}`)
      this.attributes = await this.getEntities()
    },
    onEditAttribute(a) {
      this.$router.push({name: 'edit-attribute', params: {id: a.id}})
    },
    async onManageAttribute(a) {
      await this.$router.push({name: 'manage-attribute', params: {id: a.id}})
    }
  }
};
</script>
 
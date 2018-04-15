<template>
  <main>
    <div class="row p-1 justify-content-md-center">
      <div class="col-md-auto">
        <button type="button" class="btn btn-primary btn-lg btn-block" @click="onNewEntity">Add an entity</button>
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
    };
  },
  async mounted () {
    this.entities = await this.getEntities()
  },
  components: {
    Dropdown: () => import('@/components/Controls/Dropdown')
  },
  computed: {
  },
  methods: {
    onNewEntity () {
    },
    async getEntities() {
      var res = await this.$http.get(`${process.env.BACKEND}/api/entities`)
      return res.data
    }
  }
};
</script>
 
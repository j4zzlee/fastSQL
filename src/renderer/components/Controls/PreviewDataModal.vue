<template>
<Modal :title="'Preview'" :isShow="isShow" :canClose="true" :onHide="onClosePreview">
  <table slot="body" class="table table-striped" v-if="data && data.length">
    <thead>
      <tr>
        <th scope="col" v-for="key in columns" v-bind:key="key">
          {{/^[Ii][dD]$/gi.test(key) ? '#' : key}}
        </th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="d in data" v-bind:key="getKey(d)">
        <th scope="row" v-if="/^[Ii][dD]$/gi.test(key)" v-for="key in columns" v-bind:key="key">
          {{d[key]}}
        </th>
        <td v-if="!(/^[Ii][dD]$/gi.test(key))" v-for="key in columns" v-bind:key="key">
          {{d[key]}}
        </td>
      </tr>
    </tbody>
  </table>
  <div slot="footer" class="form-group">
    <button class="form-control btn btn-primary" @click="onClosePreview">OK</button>
  </div>
</Modal>
</template>
<script>
export default {
  name: 'preview-data-modal',
  props: {
    data: Array,
    isShow: Boolean,
    onClose: Function
  },
  data() {
    return {
    }
  },
  async created() {},
  async mounted() {
  },
  watch: {},
  components: {
    Modal: () => import('@/components/Controls/Modal'),
    DynamicOption: () => import('@/components/Controls/DynamicOption')
  },
  computed: {
    columns() {
      let result = []
      if (this.data && this.data.length) {
        result = Object.keys(this.data[0])
      }
      return result
    }
  },
  methods: {
    getKey(d) {
      let result = ''
      for (var i = 0; i < this.columns.length; i++) {
        result += ';' + d[this.columns[i]]
      }
      return result
    },
    onClosePreview() {
      this.onClose && this.onClose()
    }
  }
}
</script>
 
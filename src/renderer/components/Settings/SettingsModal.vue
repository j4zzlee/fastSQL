<template>
  <Modal :title="'Settings'" :isShow="showSettings" :canClose="false">
    <div slot="body">
      <div class="alert alert-danger" v-if="errorMessage">{{errorMessage}}</div>
      <DynamicOption v-bind:key="option.Name"
        v-for="option in options"
        :option="option"
        @change="(val) => option.value = val"></DynamicOption>
    </div>
    <div slot="footer">
      <button type="button" class="btn btn-primary" @click="onConnect">
        <span class="fa fa-spinner spin" v-if="loading"></span>
        Connect</button>
    </div>
  </Modal>
</template>

<script>
import { mapState, mapMutations } from 'vuex'
export default {
  name: 'settings-modal',
  props: {
    showSettings: Boolean,
    onConnected: Function
  },
  data() {
    return {
      options: null,
      errorMessage: null,
      loading: false
    }
  },
  created() {},
  async mounted() {
    this.errorMessage = null
    // if (!this.options) {
    //   this.options = await this.getOptions()
    // }
  },
  computed: {
    ...mapState({})
  },
  events: {},
  methods: {
    ...mapMutations('Settings', ['SET_CONNECTED']),
    async onConnect() {
      try {
        this.loading = true
        this.errorMessage = null
        const res = await this.$http.post(
          `${process.env.BACKEND}/api/settings/db/connect`,
          this.options
        )
        if (res.data.success) {
          // successful
          await this.$http.post(
            `${process.env.BACKEND}/api/settings/db`,
            this.options
          )
          this.SET_CONNECTED()
          this.onConnected && this.onConnected()
        } else {
          this.errorMessage = res.data.message
        }
      } catch (err) {
        this.errorMessage = err.message
      } finally {
        this.loading = false
      }
    },
    async getOptions() {
      const result = await this.$http.get(
        `${process.env.BACKEND}/api/settings/db/options`
      )
      return result.data
    }
  },
  watch: {
    async showSettings(isShow) {
      if (isShow && !this.options) {
        this.options = await this.getOptions()
      }
    }
  },
  components: {
    Modal: () => import('@/components/Controls/Modal'),
    DynamicOption: () => import('@/components/Controls/DynamicOption')
  }
}
</script>
<template>
  <div id="app">
    <router-view></router-view>
    <SettingsModal :showSettings="showSettings" :onConnected="onConnected"></SettingsModal>
  </div>
</template>

<script>
import { mapState, mapMutations } from 'vuex'
import 'codemirror/lib/codemirror.css'
import 'codemirror/mode/sql/sql'
export default {
  name: 'fastSQL',
  data() {
    return {
      showSettings: false
    }
  },
  created() {},
  async mounted() {
    this.showSettings = false
    const res = await this.$http.post(
      `${process.env.BACKEND}/api/settings/db/connect`,
      []
    )
    if (!res.data.success) {
      this.showSettings = true
    } else {
      this.SET_CONNECTED()
    }
  },
  computed: {
    ...mapState({})
  },
  events: {},
  methods: {
    ...mapMutations('Settings', ['SET_CONNECTED']),
    onConnected() {
      this.showSettings = false
    }
  },
  watch: {},
  components: {
    SettingsModal: () => import('@/components/Settings/SettingsModal')
  }
}
</script>

<style lang="scss">
@import '../../node_modules/bootstrap/scss/bootstrap.scss';
@import './assets/styles/main.scss';
</style>

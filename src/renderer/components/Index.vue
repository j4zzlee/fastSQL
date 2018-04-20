<template>
  <main>
    <div class="form-group">
      <label for="service-file" class="font-weight-bold font-italic">Service</label>
      <span class="form-control-plaintext" id="service-file" v-if="serviceFileFound">
        <i class="text-success fa fa-check-square-o" ></i> Source service found
      </span>
      <span class="form-control-plaintext" id="service-file" v-if="!serviceFileFound">
        <i class="text-danger fa fa-exclamation-triangle" ></i> The path "{{serviceFile}}" does not exists
      </span>
    </div>
    <div class="form-group">
      <label for="service-running" class="font-weight-bold font-italic">Service</label>
      <span class="form-control-plaintext" id="service-running" v-if="serviceRunning">
        <i class="text-success fa fa-check-square-o" ></i> The service is running
      </span>
      <span class="form-control-plaintext" id="service-running" v-if="!serviceRunning">
        <i class="text-danger fa fa-exclamation-triangle" ></i> The api service is not running
      </span>
    </div>
    <div class="form-group">
      <label for="service-port" class="font-weight-bold font-italic">Service Port</label>
      <input type="text" class="form-control" v-model="conf.api.port"/>
    </div>
    <div>
      <button class="btn btn-primary" @click="onRefreshStatus">
        <span class="fa fa-spinner spin" v-if="loading"></span>
        Refresh Status
      </button>
      <button class="btn btn-primary" @click="onSave">
        Save
      </button>
    </div>
    <SettingsModal :showSettings="showSettings" :onConnected="onConnected"></SettingsModal>
  </main>
</template>

<script>
import fs from 'fs'
import path from 'path'
import { mapState, mapMutations } from 'vuex'
export default {
  name: 'home-page',
  data() {
    return {
      serviceFile: '',
      serviceFileFound: false,
      showSettings: false,
      serviceRunning: false,
      errorMessage: '',
      loading: false,
      conf: {
        api: {
          port: 7001
        }
      }
    }
  },
  async created() {
    var confPath = path.resolve(process.env.APP_DIR, 'config.json')
    if (fs.existsSync(confPath)) {
      this.conf = JSON.parse(fs.readFileSync(confPath, 'utf8'))
      process.env.BACKEND = `http://localhost:${this.conf.api.port}`
      await this.onRefreshStatus()
    }
  },
  components: {
    Dropdown: () => import('@/components/Controls/Dropdown'),
    SettingsModal: () => import('@/components/Settings/SettingsModal')
  },
  computed: {
    ...mapState({})
  },
  methods: {
    ...mapMutations('Settings', ['SET_CONNECTED']),
    onConnected() {
      this.showSettings = false
    },
    async checkServiceFile() {
      let servicePath = path.resolve(
        __dirname,
        '../../api/bin/dist/win',
        'FastSQL.Api.exe'
      )
      if (process.env.NODE_ENV !== 'development') {
        servicePath = path.resolve(
          __dirname,
          '../../dist/api/bin/dist/win',
          'FastSQL.Api.exe'
        )
      }
      this.serviceFile = servicePath
      if (fs.existsSync(servicePath)) {
        this.serviceFileFound = true
      } else {
        this.serviceFileFound = false
      }
    },
    async checkServiceRunning() {
      try {
        const res = await this.$http.post(
          `${process.env.BACKEND}/api/settings/db/connect`, // ${process.env.BACKEND} 63923
          []
        )
        if (!res.data.success) {
          this.showSettings = true
        } else {
          this.SET_CONNECTED()
        }
        this.serviceRunning = true
      } catch (err) {
        this.errorMessage = err.message
        this.serviceRunning = false
      }
    },
    async onSave() {
      try {
        var confPath = path.resolve(process.env.APP_DIR, 'config.json')
        process.env.BACKEND = `http://localhost:${this.conf.api.port}`
        fs.writeFileSync(confPath, JSON.stringify(this.conf, null, 2))
        await this.onRefreshStatus()
      } catch (err) {
        alert(err.message)
      }
    },
    async onRefreshStatus() {
      this.loading = true
      this.serviceFileFound = false
      this.serviceFile = ''
      this.errorMessage = ''
      this.serviceRunning = false
      try {
        await this.checkServiceFile()
        await this.checkServiceRunning()
      } finally {
        this.loading = false
      }
    }
  }
}
</script>
 
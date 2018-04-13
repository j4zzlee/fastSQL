<template>
  <div id="wrapper">
    <SettingsModal :showSettings="showSettings" :onConnected="onConnected"></SettingsModal>
    <TopNav :text="'fastSQL'">
      <ul class="navbar-nav mr-auto" slot="left">
        <Dropdown :text="'Global Configurations'" v-if="dbConnected">
          <router-link class="dropdown-item" :to="'global-connections'">Connections</router-link>
          <router-link class="dropdown-item" :to="'entities'">Entities</router-link>
          <router-link class="dropdown-item" :to="'attributes'">Attributes</router-link>
        </Dropdown>
        <Dropdown :text="'Queues'" v-if="dbConnected">
          <a class="dropdown-item" href="#">Action</a>
          <a class="dropdown-item" href="#">Another action</a>
          <div class="dropdown-divider"></div>
          <a class="dropdown-item" href="#">Something else here</a>
        </Dropdown>
        <Dropdown :text="'Schedules'" v-if="dbConnected">
          <a class="dropdown-item" href="#">Action</a>
          <a class="dropdown-item" href="#">Another action</a>
          <div class="dropdown-divider"></div>
          <a class="dropdown-item" href="#">Something else here</a>
        </Dropdown>
        <li class="nav-item">
          <a class="nav-link" href="#" @click="() => showSettings = !showSettings">Settings</a>
        </li>
      </ul>
      <form class="form-inline my-2 my-lg-0" slot="right">
        <input class="form-control mr-sm-2" type="search" placeholder="Search" aria-label="Search">
        <button class="btn btn-outline-success my-2 my-sm-0" type="submit">Search</button>
      </form>
    </TopNav>
    <router-view></router-view>
  </div>
</template>

<script>
import { mapState } from 'vuex'
export default {
  name: 'layout-page',
  data() {
    return {
      showSettings: false
    }
  },
  components: {
    Dropdown: () => import('@/components/Controls/Dropdown'),
    TopNav: () => import('@/components/Controls/TopNav'),
    SettingsModal: () => import('@/components/Settings/SettingsModal')
  },
  computed: {
    ...mapState({
      dbConnected: state => state.Settings.db.connected
    })
  },
  methods: {
    onConnected() {
      this.showSettings = false
    }
  }
}
</script>
 
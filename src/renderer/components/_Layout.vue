<template>
  <div id="wrapper">
    <TopNav :text="'fastSQL'">
      <ul class="navbar-nav mr-auto" slot="left">
        <Dropdown :text="'Global Configurations'">
          <router-link class="dropdown-item" :to="'global-connections'">Connections</router-link>
          <router-link class="dropdown-item" :to="'entities'">Entities</router-link>
          <router-link class="dropdown-item" :to="'attributes'">Attributes</router-link>
        </Dropdown>
        <Dropdown :text="'Queues'">
          <a class="dropdown-item" href="#">Action</a>
          <a class="dropdown-item" href="#">Another action</a>
          <div class="dropdown-divider"></div>
          <a class="dropdown-item" href="#">Something else here</a>
        </Dropdown>
        <Dropdown :text="'Schedules'">
          <a class="dropdown-item" href="#">Action</a>
          <a class="dropdown-item" href="#">Another action</a>
          <div class="dropdown-divider"></div>
          <a class="dropdown-item" href="#">Something else here</a>
        </Dropdown>
        <li class="nav-item">
          <a class="nav-link" href="#">Settings</a>
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

export default {
  name: 'layout-page',
  data() {
    return {
    };
  },
  components: {
    Dropdown: () => import('@/components/Controls/Dropdown'),
    TopNav: () => import('@/components/Controls/TopNav')
  },
  computed: {
  },
  methods: {
  }
};
</script>
 
<template>
  <div id="wrapper">
    <div id="sidenav" class="sidenav active" ref="sidenav">
      <a href="#"
        class="actions"
        @click="onConnect"
        data-toggle="tooltip"
        title="New connection">+</a>
    </div>
    <main>

    </main>
  </div>
</template>

<script>
import { mapState, mapActions, mapMutations } from 'vuex';
export default {
  name: 'home-page',
  data () {
    return {
      isOpenNav: false
    };
  },
  async created() {
    await this.loadConnections();
  },
  components: {
    ConnectionModal: () => import('@/components/Connection/NewConnection')
  },
  computed: {
    ...mapState({
      connections: state => state.Connection.connections
    })
  },
  methods: {
    ...mapActions('Connection', ['loadConnections']),
    ...mapMutations('Modal', ['SHOW_MODAL']),
    async onConnect () {
      this.SHOW_MODAL('NewConnection');
    }
  }
};
</script>
 
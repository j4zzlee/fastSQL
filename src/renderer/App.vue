<template>
  <div id="app">
    <router-view></router-view>
    <component :is="component" v-if="modalVisible"></component>
    <div v-if="modalVisible"
      :class="`modal-backdrop fade show`"></div>
  </div>
</template>

<script>
import Vue from 'vue';
import { mapState } from 'vuex';
import 'codemirror/lib/codemirror.css';
import 'codemirror/mode/sql/sql';
export default {
  name: 'fastSQL',
  data() {
    return {
      component: null
    };
  },
  created() {},
  mounted() {
  },
  computed: {
    ...mapState({
      modalVisible: state => state.Modal.visible,
      modalName: state => state.Modal.modalName
    })
  },
  events: {},
  methods: {
    // ''
    // ...mapMutations(['HIDE_MODAL'])
  },
  watch: {
    modalName(componentName) {
      if (!componentName) return;

      Vue.component(componentName, () => import(`@/components/Connection/${componentName}`));

      this.component = componentName;
    }
  }
};
</script>

<style lang="scss">
@import './assets/styles/main.scss';
@import '../../node_modules/bootstrap/scss/bootstrap.scss';
</style>

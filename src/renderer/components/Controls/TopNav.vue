<template>
  <nav class="navbar navbar-expand-lg navbar-light bg-light">
    <a class="navbar-brand" href="#">{{text}}</a>
    <button class="navbar-toggler" type="button" 
      data-toggle="collapse" 
      data-target="#navbarSupportedContent" 
      aria-controls="navbarSupportedContent" 
      aria-expanded="false" 
      aria-label="Toggle navigation"
      @click="onToggleNav">
      <span class="navbar-toggler-icon"></span>
    </button>

    <div :class="`${isOpenNav ? '' : 'collapse'} navbar-collapse`" id="navbarSupportedContent">
      <slot name="left"></slot>
      <slot name="right"></slot>
    </div>
  </nav>
</template>

<script>

export default {
  name: 'topnav-control',
  props: {
    text: String
  },
  data() {
    return {
      isOpenNav: false
    };
  },
  components: {
  },
  computed: {
  },
  methods: {
    onToggleNav() {
      this.isOpenNav = !this.isOpenNav
    }
  }
};
</script>
 
<template>
  <li :class="`nav-item dropdown ${isOpen ? 'show' : ''}`" v-click-outside="onClose">
    <a class="nav-link dropdown-toggle"
      href="#" id="navbarDropdown" 
      role="button" 
      data-toggle="dropdown" 
      aria-haspopup="true" 
      aria-expanded="false"
      @click="onToggle">
      {{text}}
    </a>
    <div @click="onClose" :class="`dropdown-menu ${isOpen ? 'show' : ''}`" aria-labelledby="navbarDropdown">
      <slot></slot>
    </div>
  </li>
</template>

<script>

export default {
  name: 'dropdown-control',
  props: {
    text: String
  },
  data() {
    return {
      isOpen: false
    };
  },
  components: {
  },
  computed: {
  },
  methods: {
    onToggle () {
      this.isOpen = !this.isOpen
    },
    onClose () {
      this.isOpen = false
    }
  }
};
</script>
 
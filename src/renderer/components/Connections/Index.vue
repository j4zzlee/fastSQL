<template>
  <div>Under construction</div>
</template>

<script>

export default {
  name: 'global-connections-page',
  data() {
    return {
    };
  },
  async created() {
    const res = this.$http.get('http://localhost:5000/api/connections')
    console.log(res)
  },
  components: {
    Dropdown: () => import('@/components/Controls/Dropdown')
  },
  computed: {
  },
  methods: {
  }
};
</script>
 
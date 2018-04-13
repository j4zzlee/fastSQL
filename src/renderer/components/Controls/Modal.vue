<template>
  <div :class="`modal fade ${isShow ? 'show': ''}`" tabindex="-1" role="dialog" :style="{display: isShow ? 'block' : 'none'}">
    <div class="modal-dialog" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">{{title}}</h5>
          <button v-if="canClose" type="button" class="close" data-dismiss="modal" aria-label="Close" @click="onHideHandler">
            <span aria-hidden="true">&times;</span>
          </button>
        </div>
        <div class="modal-body">
          <slot name="body"></slot>
        </div>
        <div class="modal-footer">
          <slot name="footer"></slot>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'modal-control',
  props: {
    title: String,
    onHide: Function,
    isShow: Boolean,
    canClose: Boolean
  },
  data() {
    return {}
  },
  async created() {
  },
  async mounted() {
    var $element = document.getElementById('modal-backdrop')
    if (this.isShow && !$element) {
      let $body = document.getElementsByTagName('body')[0]
      let $backdrop = document.createElement('div');
      $backdrop.id = 'modal-backdrop'
      $backdrop.className = 'modal-backdrop fade show'
      $body.appendChild($backdrop)
    }
  },
  components: {},
  computed: {
  },
  methods: {
    onHideHandler() {
      this.onHide && this.onHide()
    }
  },
  watch: {
    isShow(isShow) {
      let $body = document.getElementsByTagName('body')[0]
      var $element = document.getElementById('modal-backdrop')
      if (isShow && !$element) {
        let $backdrop = document.createElement('div');
        $backdrop.id = 'modal-backdrop'
        $backdrop.className = 'modal-backdrop fade show'
        $body.appendChild($backdrop)
      } else {
        $element = document.getElementById('modal-backdrop')
        $element.parentNode.removeChild($element);
      }
    }
  }
}
</script>
 
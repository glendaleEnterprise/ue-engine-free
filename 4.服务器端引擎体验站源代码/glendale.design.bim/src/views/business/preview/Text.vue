<template>
  <div :style="{ height: `${searchHeight}px`,display: 'flex',justifyContent: 'center' }">
    <iframe style="border: 0" :style="{ height: `${searchHeight}px` ,width:'70%'}" :src="src"> </iframe>
  </div>
</template>

<script>
import { getFileByBlobName, getDocumentById } from '@/api/file'
export default {
  name: 'Text',
  components: {},
  props: {
    fileName: {
      type: String,
      default: '',
    },
    share: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      searchHeight: 0,
      src: '',
    }
  },
  mounted() {
    this.src =  this.fileName
    this.onResize()
    window.onresize = () => {
      this.onResize()
    }
  },
  destroyed() {
    // 组件销毁后解绑事件
    window.onresize = null
  },
  methods: {
    onResize() {
      if (this.share) {
        this.searchHeight = window.innerHeight
      } else {
        this.searchHeight = window.innerHeight - 128
      }
    },
  },
}
</script>
 

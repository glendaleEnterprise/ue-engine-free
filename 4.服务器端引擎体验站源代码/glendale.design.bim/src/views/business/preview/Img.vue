<template>
  <viewer
    ref="viewer"
    :options="options"
    :images="images"
    rebuild
    class="img_viewer"
    @inited="inited"
    :style="{ height: `${searchHeight}px` }"
  >
    <template #default="scope">
      <img
        v-for="src in scope.images"
        :src="src"
        :key="src"
        :style="{ maxHeight: `${searchHeight}px` }"
      />
    </template>
  </viewer>
</template>

<script>
import 'viewerjs/dist/viewer.css'
import { component as Viewer } from 'v-viewer'
import { getDocumentById } from '@/api/file'

export default {
  name: 'Img',
  components: {
    Viewer,
  },
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
      images: [],
      options: {
        inline: false,
        button: true,
        navbar: false,
        title: false,
        toolbar: false,
        tooltip: false,
        movable: true,
        zoomable: true,
        rotatable: true,
        scalable: true,
        transition: true,
        fullscreen: true,
      },
    }
  },  
  mounted() {     
    const url = this.fileName
    this.images = [url]
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
    inited(viewer) {
      this.$viewer = viewer
    },
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
<style lang="less" scoped>
.img_viewer {
  cursor: pointer;   
  // background-color: #87CEFA; 
  img {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    -webkit-transform: translate(-50%, -50%);
    -moz-transform: translate(-50%, -50%);
    max-width: 90%;
  }
}
</style>

<template>   
  <div class="video_viewer" :style="{height: `${searchHeight}px` }">
    <video
      :src="soundsrc"
      autoplay="autoplay"
      muted="muted"
      style="width: 100%; height: 100%"
      controls="controls"
    ></video>
  </div>
</template>
<script>
import 'viewerjs/dist/viewer.css'
import { component as Viewer } from 'v-viewer'
import { getFileByBlobName, getDocumentById } from '@/api/file'
import { getVideoPath } from '@/api/document'

export default {
  name: 'Video',
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
      soundsrc: '',      
    }
  },
  mounted() {
    const that = this
    that.soundsrc =  this.fileName
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
    getVideo(url) {
      //获取视频
      const that = this
      //创建XMLHttpRequest对象
      var xhr = new XMLHttpRequest()
      //配置请求方式、请求地址以及是否同步
      xhr.open('get', url, true)
      //设置请求结果类型为blob
      xhr.responseType = 'blob'
      //请求成功回调函数
      xhr.onload = function () {
        if (this.status == 200) {
          //请求成功
          //获取blob对象
          var blob = this.response //获取blob对象地址，并把值赋给容器
          that.soundsrc = URL.createObjectURL(blob)
        }
      }
      xhr.send()
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
.video_viewer {
  background-color: #87cefa;
}
</style>

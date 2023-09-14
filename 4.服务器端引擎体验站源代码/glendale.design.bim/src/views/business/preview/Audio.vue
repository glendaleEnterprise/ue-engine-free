<template>
  <div class="audio_content" :style="{height: `${searchHeight}px` }">
    <template>
      <audio controls :src="soundsrc" autoplay="autoplay" muted="muted" loop>
        您的浏览器不支持 audio 元素。type="audio/mpeg"
      </audio>
    </template>
  </div>
</template>
<script>
import { getDocumentById } from '@/api/file'
export default {
  name: 'Video',  
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
  beforeMount() {
    const that = this
    this.$nextTick(function () {
      that.soundsrc = this.fileName
      // that.getVideo(url)
    })
  },
  mounted() {
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
.audio_content {
  background-color: #87cefa; 
  text-align: center;
  padding-top: 20%;
}
</style>

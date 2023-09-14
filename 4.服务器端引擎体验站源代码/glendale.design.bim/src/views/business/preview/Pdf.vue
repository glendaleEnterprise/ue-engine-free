<template>
  <a-spin :spinning="loading" tip="正在打开资料，请稍后...">
    <div class="box" :style="{ height: `${searchHeight}px` }">
      <div class="content" :style="{ width: `${scale}%` }">
        <iframe
          :src="url"
          id="word-iframe"
          style="width: 100%;  border: 0"
          :style="{ height: searchHeight + 'px' }"
        ></iframe>
      </div>
      <div class="control">
        <a @click="magnify"><a-icon type="zoom-in" /></a>
        <a @click="shrink"><a-icon type="zoom-out" /></a>
        <a @click="primeval"><div class="primeval">1:1</div></a>
      </div>
    </div>
  </a-spin>
</template>

<script>
import pdf from 'vue-pdf'
import { getDocumentById } from '@/api/file'

export default {
  name: 'Pdf',
  components: {
    pdf,
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
      url: '',
      numPages: 0,
      scale: 70,
      scaleAdd: 10,
      loading: true,
    }
  },
  created() {
    // const path = getDocumentById(this.fileName)
    this.url =this.fileName
  },

  mounted() {
    this.numPages = [ this.url]
    this.loading = false
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
    //放大
    magnify() {
      let { scale } = this
      scale += this.scaleAdd
      scale = scale > 1000 ? 1000 : scale
      this.scale = scale
    },
    //回到原始
    primeval() {
      this.scale = 50
    },
    //缩小
    shrink() {
      let { scale } = this
      scale -= this.scaleAdd
      scale = scale < 10 ? 10 : scale
      this.scale = scale
    },
  },
}
</script>
<style lang="less" scoped>
.box {
  background-color: rgba(0, 0, 0, 0.1);
  overflow: auto;
  .content {
    width: 65%;
    margin: 0 auto;
  }
}
.control {
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: rgba(0, 0, 0, 0.4);
  padding: 3px;
  border-radius: 17px;
  width: 120px;
  position: absolute;
  left: 50%;
  margin-left: -60px;
  bottom: 5px;
  a {
    margin: 0 8px;
    color: #fff;
    font-size: 15px;
  }
  a:hover {
    color: #222;
  }
  .primeval {
    font-size: 8px;
    border: 1px solid #dfdfdf;
    height: 13px;
    margin-top: 1px;
    line-height: 10px;     
  }
  .primeval:hover {
    border-color: #555;
  }
}
</style>

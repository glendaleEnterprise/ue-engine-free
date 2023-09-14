<template>
  <a-spin :spinning="loading" tip="正在加载资料，请稍后...">
    <div class="office-content" :style="{ height: searchHeight + 'px' }">
      <iframe
        :srcdoc="html"
        id="word-iframe"
        style="width: 100%;  border: 0"
        :style="{ height: searchHeight + 'px' }"
      ></iframe>
      <div v-if="total > 1" style="position: absolute; right: 40px; bottom: 24px">
        <a-button :disabled="prev" @click="prevChange" type="primary">上一页</a-button>&nbsp;&nbsp;&nbsp;
        <a-button :disabled="next" @click="nextChange" type="primary">下一页</a-button>
      </div>
    </div>
  </a-spin>
</template>

<script>
import { getWordHtml } from '@/api/document'

export default {
  name: 'Word',
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
      loading: true,
      html: undefined,
      total: 0,
      prev: true,
      next: true,
      searchWidth: 0,
      searchHeight: 0,
      currentPage: 0,
    }
  },

  async created() {
    const that = this
    this.getData(this.fileName, 0)
  },
  mounted() {
    this.onResize()
    window.onresize = () => {
      this.onResize()
    }
  },
  methods: {
    async getData(fileName, currentPage) {
      this.loading = true       
      getWordHtml(fileName, currentPage)
        .then((result) => {
          this.html = result.result.replace(this.getOtherHtml(), '')
          this.total = result.total
          if (this.currentPage == 0) {
            this.prev = true
            this.next = false
          } else if (this.currentPage == this.total - 1) {
            this.prev = false
            this.next = true
          } else {
            this.prev = false
            this.next = false
          }
          this.loading = false
        })
        .catch(() => {
          this.html = '文档加载异常，请稍后再试'
          this.loading = false
        })
    },
    onResize() {
      if (this.share) {
        this.searchHeight = window.innerHeight
      } else {
        this.searchHeight = window.innerHeight - 128
      }
    },
    getOtherHtml() {
      return `Evaluation Warning: The document was created with Spire.Doc for .NET.`
    },
    prevChange() {
      this.prev = false
      this.currentPage = this.currentPage - 1
      this.getData(this.fileName, this.currentPage)
    },
    nextChange() {
      this.next = false
      this.currentPage = this.currentPage + 1
      this.getData(this.fileName, this.currentPage)
    },
  },
}
</script>

<style lang="less" scoped>
.office-content{
    background: #fff; 
} 
</style>

<template>
  <a-spin :spinning="loading" tip="正在打开资料，请稍后...">
    <div class="office-content" :style="{ height: searchHeight + 'px' }">
      <iframe
        :srcdoc="html"
        id="iframe1"
        style="width: 100%; border: 0"
        :style="{ height: searchHeight + 'px' }"
      ></iframe>
    </div>
  </a-spin>
</template>

<script>
import { getExcelHtml } from '@/api/document'
export default {
  name: 'Settings',
  components: {},
  data() {
    return {
      loading: true,
      html: undefined,
      searchWidth: 0,
      searchHeight: 0,
    }
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
  mounted() {    
    this.getData(this.fileName)
    this.loaded()
    this.onResize()
    window.onresize = () => {
      this.onResize()
    }
  },
  methods: {
    async getData(id) {
      await getExcelHtml(id).then((result) => {
        this.html = result.replace(this.getOtherHtml(), '')
        //this.loading = false
      })
    },
    onResize() {
      if (this.share) {
        this.searchHeight = window.innerHeight
      } else {
        this.searchHeight = window.innerHeight - 128
      }
    },
    loaded() {
      var that = this
      const iframe = document.querySelector('#iframe1')
      // 处理兼容行问题
      if (iframe.attachEvent) {
        iframe.attachEvent('onload', function () {         
          that.loading = false
        })
      } else {
        iframe.onload = function () {          
          that.loading = false
        }
      }
    },
    getOtherHtml() {
      return `<div class="pt-000000"><p dir="ltr" class="pt-Normal"><span class="pt-DefaultParagraphFont">Evaluation Warning: The document was created with Spire.Doc for .NET.</span></p><p dir="ltr" class="pt-Normal-000001"><span xml:space="preserve" class="pt-000002"> </span></p><p dir="ltr" class="pt-Normal-000001"><span xml:space="preserve" class="pt-000002"> </span></p><p dir="ltr" class="pt-Normal-000001"><span xml:space="preserve" class="pt-000002"> </span></p></div>`
    },
  },
}
</script>

<style lang="less" scoped>
// .office-content {
//   background: #fff;
// }
</style>

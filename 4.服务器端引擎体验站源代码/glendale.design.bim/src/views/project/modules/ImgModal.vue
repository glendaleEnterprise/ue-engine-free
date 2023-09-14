<template>
  <a-modal v-model="statisticsshow" title="图片详情" width="800px" class="modal" :footer="null">
    <div class="imgCtrl">
      <div v-for="(file, i) in imgdata" :key="i">
        <a :href="getFileByBlobName(file.blobName)" target="_blank">
          <img :src="getFileByBlobName(file.blobName)"/>
        </a>
      </div>
    </div>
  </a-modal>
</template>
 
<script>
import { mapGetters } from 'vuex' 
import { getFileByBlobName } from '@/api/file'
export default {
  name: 'ImgModal',
  components: {
  },
  computed: {
  },
   props: {
    imgdata:[]
  },
  data () {
    return {
      statisticsshow:true,
      previewVisible: false,
      previewImage: '',
    }
  },
  async created () {
   
  },
  async mounted () { 
  },
  methods: {
    getFileByBlobName,
    async see(record){
      if (record.id) {
        const model = await getDataById(record.id)
        this.fromData = { ...model }
      } else {
        this.fromData = { ...record }
      }
      this.visible = true
    },
    handleCancel() {
      this.previewVisible = false
    },
    async handlePreview(file) {
      if (!file.url && !file.preview) {
        file.preview = await getBase64(file.originFileObj)
      }
      this.previewImage = file.url || file.preview
      this.previewVisible = true
    },
    handleChange({ fileList }) {
      this.fileList = fileList
    },
    statisticsshowclick(){
        this.statisticsshow=true
    }
  },
}
</script>
<style scoped lang="less">
/deep/#chart .canvas{
    left: 200px;
}
.modalbox{
  width: 160px;
  float: left;
}
.modal_table{
    display: flex !important;
    border: 1px solid #333;
    margin: 0 !important;
    border-bottom: 0;
    padding: 0 !important;
    
}
.modal_table:last-child{
    border-bottom: 1px solid #333
}
.modal_table li{
    width: 80px;
    height: 25px;
    line-height: 25px;
    text-align: center;
}
.modal_table li:last-child{
    border-left: 1px solid #333;
} 
.imgCtrl{height: 450px; overflow: auto;}
.imgCtrl div{float:left;width: 190px; height: 200px; margin: 5px 25px 5px 25px;}
.imgCtrl div a{text-decoration: none;background-color: transparent;outline: none;cursor: pointer;-webkit-transition: color 0.3s;transition: color 0.3s;-webkit-text-decoration-skip: objects;}
.imgCtrl div a img{position: static;display: block;width: 100%;height: 100%;-o-object-fit: cover;object-fit: cover;}
</style>

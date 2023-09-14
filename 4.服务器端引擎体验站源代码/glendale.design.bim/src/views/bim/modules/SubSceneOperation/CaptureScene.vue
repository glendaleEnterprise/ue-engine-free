<template>
  <div ref="wrap" :class="{ 'mobile-Model':isMobile  }">
    <a-card size="small" :bordered="false" class="card-custom-style">
      <a-form-model class="roaming-set scroll-box" ref="ruleForm" :hideRequiredMark="true" @cancel="handleCancel">
        <a-form-item :wrapper-col="{ span: 24 }" style="margin-bottom: 20px;">
          <a-space :size="20">
            <a-button ghost @click="CaptureSceneImages" @keyup.prevent @keydown.enter.prevent>截取场景图片</a-button>
          </a-space>
        </a-form-item>
        <a-form-item label="预览" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }" v-show="previewImage">
          <img alt="example" style="width: 100%" :src="previewImage" />
        </a-form-item>
        <a-form-item :wrapper-col="{ span: 24 }" v-show="previewImage" style="margin-top: 16px;">
          <a-space :size="20">
            <a-button ghost @click="DownloadImages" @keyup.prevent @keydown.enter.prevent>下载图片</a-button>
            <a-button ghost @click="SetProjectCover" v-if="projectMessage.functionalPermissions" @keyup.prevent
              @keydown.enter.prevent>项目封面</a-button>
          </a-space>
        </a-form-item>
      </a-form-model>
    </a-card>
  </div>
</template>
<script>
  import {
    uploadFile,
    getFileByBlobName
  } from '@/api/file'
  import {
    modelProjectCover
  } from '@/api/document'
  import {
    combineProjectCover
  } from '@/api/combine'
  import {
    _isMobile
  } from '@/api/public'
  export default {
    name: 'CaptureScene',
    props: {
      projectMessage: {
        type: Object,
        default: null,
      },
    },
    data() {
      return {
        previewImage: '',
        imgBlobName: '',
        isMobile: false,
      }
    },
    mounted() {
      this.isMobile = _isMobile() ? true : false;
    },
    methods: {
      CaptureSceneImages() {
        const that = this;
        api.Public.saveScreenShot((res) => {
          that.previewImage = res;
          const uploadImg = that.convertBase64UrlToBlob(res)
          uploadFile(uploadImg)
            .then((res) => {
              that.imgBlobName = res;
              var url = getFileByBlobName(res)
            })
            .catch((err) => {
              that.$message.error('图片截取失败，请重试！')
            })
        })
      },
      convertBase64UrlToBlob(urlData) {
        const fd = new FormData()
        var bytes = window.atob(urlData.split(',')[1]) //去掉url的头，并转换为byte
        //处理异常,将ascii码小于0的转换为大于0
        var ab = new ArrayBuffer(bytes.length)
        var ia = new Uint8Array(ab)
        for (var i = 0; i < bytes.length; i++) {
          ia[i] = bytes.charCodeAt(i)
        }
        const blob = new Blob([ab], {
          type: 'image/png'
        })
        fd.append('file', blob, new Date().getTime() + '.png')
        return fd
      },
      handleCancel() {
        this.$notification.destroy();
      },
      DownloadImages() {
        if (this.previewImage == '') {
          this.$message.warning('请先点击截取场景图片！')
          return false;
        }
        var base64 = this.previewImage.toString(); // imgSrc 就是base64哈
        var byteCharacters = atob(
          base64.replace(/^data:image\/(png|jpeg|jpg);base64,/, "")
        );
        var byteNumbers = new Array(byteCharacters.length);
        for (var i = 0; i < byteCharacters.length; i++) {
          byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        var byteArray = new Uint8Array(byteNumbers);
        var blob = new Blob([byteArray], {
          type: undefined,
        });
        var aLink = document.createElement("a");
        aLink.download = "CaptureScene.png"; //这里写保存时的图片名称
        aLink.href = URL.createObjectURL(blob);
        aLink.click();
      },
      SetProjectCover() {
        const that = this;
        if (that.projectMessage.sceneType == 0) { //单模型
          modelProjectCover(that.projectMessage.id, that.imgBlobName).then((res) => {
            if (res.blobName) {
              that.$message.success('项目封面设置成功！')
            }
          })
        } else {
          combineProjectCover(that.projectMessage.id, that.imgBlobName).then((res) => {
            if (res.blobName) {
              that.$message.success('项目封面设置成功！')
            }
          })
        }
      }
    },
  }
</script>
<style lang="less" scoped></style>
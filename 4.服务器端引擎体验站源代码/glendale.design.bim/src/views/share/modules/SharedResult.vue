<template>
  <a-modal :visible="sharedResultState" :label-col="labelCol" :wrapper-col="wrapperCol" class="form-box save-view"
    title="链接分享" :maskClosable="false" :width="900" @cancel="handleCancel">
    <a-descriptions>
      <a-descriptions-item :span="3" label="分享名称"> {{ sharedResult.shareTile }} </a-descriptions-item>
      <a-descriptions-item :span="3" v-if="sharedResult.effectiveDay === 200" label="有效期(天)">永久有效</a-descriptions-item>
      <a-descriptions-item :span="3" v-else label="有效期(天)">{{ sharedResult.effectiveDay }}</a-descriptions-item>
      <a-descriptions-item :span="3" v-if="sharedResult.isVerify" label="授权码">{{
        sharedResult.auth
      }}</a-descriptions-item>

      <!-- <a-descriptions-item :span="3" v-if="sharedResult.shareCount === 0" label="分享次数(次)">无限</a-descriptions-item>
      <a-descriptions-item :span="3" v-else label="分享次数(次)">{{ sharedResult.shareCount }}</a-descriptions-item> -->

      <a-descriptions-item :span="3" label="分享链接">{{ sharedLink }} </a-descriptions-item>
    </a-descriptions>
    <div class="qrcode" id="foo">
      <vue-qr :text="sharedLink" :margin="0" :size="100" colorLight="#fff" :logoSrc="logoUrl"></vue-qr>
    </div>
    <template slot="footer">
      <a-button-group>
        <a-button v-if="!sharedResult.isVerify" v-clipboard:copy="shareTitle + '\n' + sharedLink + copySign"
          v-clipboard:success="onCopy" v-clipboard:error="onError">复制分享信息</a-button>
        <a-button v-else v-clipboard:copy="shareTitle + '\n' + sharedLink + '\n授权码：' + sharedResult.auth + copySign"
          v-clipboard:success="onCopy" v-clipboard:error="onError">复制分享信息</a-button>
        <a-button v-clipboard:copy="sharedLink" v-clipboard:success="onCopy" v-clipboard:error="onError">复制分享链接</a-button>
        <a-button v-if="false" @click="copyQr" title="如果不能复制请截屏">复制二维码</a-button>
      </a-button-group>
    </template>
  </a-modal>
</template>
<script>
import logoUrl from '../../../static/img/logo-head.png'
import vueQr from 'vue-qr'
import html2canvas from 'html2canvas';
export default {
  name: 'ShareResult',
  props: {
    shareInfo: {
      type: Object,
      default: null,
    },
    visible: {
      type: Boolean,
      default: false,
    },
  },
  components: {
    vueQr,
  },
  data() {
    return {
      shareTitle: this.shareInfo.shareTile,
      sharedLink: this.shareInfo.shareLink,
      logoUrl: logoUrl,
      copySign: '\n' + process.env.VUE_APP_COPY_SIGN,
      labelCol: { span: 7 },
      wrapperCol: { span: 16 },
      sharedResult: this.shareInfo,
      sharedResultState: this.visible,
    }
  },
  mounted() {
  },
  methods: {
    handleCancel() {
      const that = this
      that.sharedResultState = false
      this.$emit('changeShare', false)
    },
    onCopy(e) {
      this.$message.success('内容已复制到剪切板！')
    },
    // 复制失败时的回调函数
    onError(e) {
      this.$message.error('抱歉，复制失败！')
    },
    copyQr() {
      try {
        html2canvas(document.getElementById('foo')).then(async (canvas) => {
          this.imgUrl = canvas.toDataURL();
          const data = await fetch(this.imgUrl);
          const blob = await data.blob();

          await navigator.clipboard.write([
            // eslint-disable-next-line no-undef
            new ClipboardItem({
              [blob.type]: blob,
            }),
          ]);
          this.$message.success('内容已复制到剪切板！')
        });
      } catch (e) {
        this.$message.error('抱歉，复制失败！')
      }
    },
  },
}
</script>
<style lang="less" scoped>
.save-view {

  /deep/.ant-modal-mask,
  /deep/.ant-modal-wrap {
    z-index: 1010;
  }
}

/deep/.ant-descriptions-item-label {
  width: 140px;
  text-align: right;
}

.qrcode {
  overflow: hidden;
  width: 100px;
  height: 100px;
  position: absolute;
  float: left;
  top: 80px;
  right: 80px;
}

.daySet {
  margin-left: 90px;
}

.qrcode .q_icon {
  position: absolute;
  width: 25px;
  height: 25px;
  right: 0px;
  left: 0px;
  top: 0px;
  bottom: 0px;
  margin: auto;
  background-color: #fff;
  border: solid 1px rgb(47, 151, 251);
}

.qrcode .q_icon img {
  width: 100%;
  height: 100%;
}
</style>

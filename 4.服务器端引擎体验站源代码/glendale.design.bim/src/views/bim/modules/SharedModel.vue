<template>
  <div>
    <a-modal
      v-model="sharedState"
      class="form-box save-view"
      :title="sharedMessage.title"
      @cancel="resetSharedForm"
      @ok="onSharedSubmit"
      :width="500"
      :ok-text="$t('public.OK')"
      :cancel-text="$t('public.cancel')"
    >
      <a-form-model
        ref="ruleSharedForm"
        :model="sharedForm"
        :rules="rules"
        :label-col="labelCol"
        :wrapper-col="wrapperCol"
      >
        <a-form-model-item ref="name" label="分享名称" prop="name">
          <a-input v-model="sharedForm.name" />
        </a-form-model-item>
        <a-form-model-item ref="isVerify" label="授权方式" prop="isVerify">
          <a-switch checked-children="有授权码" un-checked-children="无授权码" v-model="sharedForm.isVerify" />
        </a-form-model-item>
        <a-form-model-item label="分享期限">
          <a-radio-group default-value="200" @change="onSetTermType" button-style="solid">
            <a-radio-button value="200"> 永久 </a-radio-button>
            <a-radio-button value="1"> 限天 </a-radio-button>
          </a-radio-group>
        </a-form-model-item>
        <div class="daySet">
          <a-radio-group v-model="sharedForm.effectiveDay" v-show="boxDaySet" @change="onDaySet">
            <a-radio :value="30"> 30天 </a-radio>
            <a-radio :value="7"> 7天 </a-radio>
            <a-radio :value="1"> 1天 </a-radio>
          </a-radio-group>
        </div>
        <!-- <a-form-model-item label="分享次数">
          <a-radio-group default-value="0" @change="onSetShareCount" button-style="solid">
            <a-radio-button value="0"> 无限 </a-radio-button>
            <a-radio-button value="1"> 限次 </a-radio-button>
          </a-radio-group>
        </a-form-model-item>
        <div class="daySet">
          <a-input-number
            :min="1"
            :formatter="value => `${value}次`"
            :parser="value => value.replace('次', '')"
            v-model='sharedForm.shareCount'
            v-show="boxCountSet"
          ></a-input-number>
        </div> -->
        <!-- <a-form-model-item ref="validityPeriod" label="有效期(天)" prop="validityPeriod">
          <a-input-number id="inputNumber" v-model="sharedForm.validityPeriod"/>
        </a-form-model-item> -->
      </a-form-model>
    </a-modal>

    <a-modal
      v-model="sharedResultState"
      :label-col="labelCol"
      :wrapper-col="wrapperCol"
      class="form-box save-view"
      title="链接分享"
      :maskClosable="false"
      :width="900"
    >
      <a-descriptions>
        <a-descriptions-item :span="3" label="分享名称"> {{ sharedResult.shareTile }} </a-descriptions-item>

        <!-- <a-descriptions-item  span="4"  label="授权方式">
      <span v-if="sharedResult.isVerify">有授权码</span>
          <span v-else>无授权码</span>
    </a-descriptions-item> -->
        <a-descriptions-item :span="3" v-if="sharedResult.effectiveDay === 200" label="有效期(天)"
          >永久有效</a-descriptions-item
        >
        <a-descriptions-item :span="3" v-else label="有效期(天)">{{ sharedResult.effectiveDay }}</a-descriptions-item>
        <a-descriptions-item :span="3" v-if="sharedResult.isVerify" label="授权码">{{
          sharedResult.auth
        }}</a-descriptions-item>

        <!-- <a-descriptions-item :span="3" v-if="sharedResult.shareCount ===0" label="分享次数(次)">无限</a-descriptions-item>
        <a-descriptions-item :span="3" v-else label="分享次数(次)">{{ sharedResult.shareCount }}</a-descriptions-item> -->
        <a-descriptions-item :span="3" label="分享链接">{{ sharedLink }} </a-descriptions-item>
      </a-descriptions>
      <div class="qrcode" >
        <vue-qr id="foo" :text="sharedLink" :margin="0" :size="100" colorLight="#fff" :logoSrc="logoUrl"></vue-qr>
      </div>
      <template slot="footer">
        <a-button-group>
          <a-button            
            v-if="!sharedResult.isVerify"           
            v-clipboard:copy="shareTitle+'\n'+sharedLink + copySign"
            v-clipboard:success="onCopy"
            v-clipboard:error="onError"
            >复制分享信息</a-button
          >
          <a-button            
            v-else            
            v-clipboard:copy="shareTitle+'\n'+sharedLink + '\n授权码：' + sharedResult.auth + copySign"
            v-clipboard:success="onCopy"
            v-clipboard:error="onError"
            >复制分享信息</a-button
          >
          <a-button
            v-clipboard:copy="sharedLink"
            v-clipboard:success="onCopy"
            v-clipboard:error="onError">复制分享链接</a-button>
<!--        <a-button @click="copyQr" title="如果不能复制请截屏">复制二维码</a-button>-->
        </a-button-group>
      </template>
    </a-modal>
  </div>
</template>

<script>
import logoUrl from '../../../static/img/logo-head.png'
import vueQr from 'vue-qr'
import { setSharedUrlSave } from '@/api/share'
export default {
  props: {
    sharedFatherState: {
      type: Boolean,
      default: false,
    },
    sharedMessage: {
      type: Object,
      default: null,
    },
    projectMessage: {
      type: Object,
      default: null,
    },
  },
  watch: {
    sharedState(newVal) {
      this.$emit('update:sharedFatherState', newVal)
    },
    sharedFatherState(newVal) {
      this.sharedState = newVal
      this.sharedForm.name = this.sharedMessage.name
      this.shareTitle=this.sharedResult.shareTile
    },
  },
  components: {
    vueQr,
  },
  data() {
    return {
      shareTitle:'',
      //sharedLink: this.shareInfo.shareLink,
      copySign: '\n' + process.env.VUE_APP_COPY_SIGN,
      logoUrl: logoUrl,
      setTermType: 'a', //设置期限类型
      boxDaySet: false, //分析期限显示
      boxCountSet: false, ///次数显示
      sharedState: this.sharedFatherState,
      sharedForm: {
        name: '',
        shareType: 1,
        effectiveDay: 200,
        isVerify: true,
        shareCount:0
      },
      rules: {
        name: [{ required: true, message: '请填写分享名称', trigger: 'change' }],
      },
      labelCol: { span: 5 },
      wrapperCol: { span: 18 },
      sharedResult: {},
      sharedResultState: false,
      sharedLink: '',
    }
  },

  methods: {
    onSetTermType(e) {
      const that = this
      if (e.target.value == 1) {
        that.boxDaySet = true
        that.sharedForm.effectiveDay = 30
      } else {
        that.sharedForm.effectiveDay = 200
        that.boxDaySet = false
      }
    },
    onSetShareCount(e){
      if (e.target.value == 0) {
        this.sharedForm.shareCount = 0
        this.boxCountSet = false
        // this.sharedForm.effectiveDay = 30
      } else {
        this.sharedForm.shareCount = 1
        this.boxCountSet = true
      }

    },
    onDaySet() {
      const that = this
    },
    onSharedSubmit() {
      const that = this
      that.$refs.ruleSharedForm.validate((valid) => {
        if (valid) {
          let parame = {
            shareTile: that.sharedForm.name,
            shareType: that.sharedMessage.shareType,
            sceneType: that.projectMessage.sceneType,
            shareCount: that.sharedForm.shareCount,
            isVerify: that.sharedForm.isVerify,
            effectiveDay: that.sharedForm.effectiveDay,
            projectId: that.projectMessage.projectId,
            modelId: that.projectMessage.id,
            details: [
              {
                relevancyId: that.sharedMessage.id,
                sort: 0,
              },
            ],
          }
          setSharedUrlSave(parame).then((res) => {
            that.sharedResult = res
            that.sharedLink = location.origin + '/#/' + res.shareLink
            that.sharedResultState = true
            that.sharedState = false
          })
        } else {
          return false
        }
      })
    },
    resetSharedForm() {
      const that = this
      that.sharedState = false
      that.sharedResultState = false
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
        window.getSelection().removeAllRanges()
        var range = document.createRange()
        range.selectNode(document.getElementById('foo'))
        window.getSelection().addRange(range)
        document.execCommand('Copy')
        this.$message.success('内容已复制到剪切板！')
      } catch (e) {
        this.$message.error('抱歉，复制失败！')
      }
    },
  },
}
</script>
<style lang="less" scoped>
  .save-view{
    /deep/.ant-modal-mask,
    /deep/.ant-modal-wrap{
      z-index: 1010;
    }
  }
 /deep/.ant-descriptions-item-label{ width: 140px; text-align: right;}
.qrcode{ overflow: hidden; width:100px; height: 100px; position: absolute; float:left; top: 80px;right: 80px;}
.daySet{margin:-15px 0 10px 94px}

.qrcode .q_icon{
    position:absolute;
    width: 25px;
    height: 25px;
    right: 0px;
    left: 0px;
    top: 0px;
    bottom: 0px;
    margin: auto;
    background-color: #fff;
    border: solid 1px rgb(47,151,251);
}
.qrcode .q_icon img{ width: 100%; height: 100%;}
</style>

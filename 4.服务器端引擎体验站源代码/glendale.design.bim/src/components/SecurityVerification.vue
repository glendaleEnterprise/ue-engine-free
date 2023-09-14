<template>
  <a-modal title="请完成安全验证" :width="400" :visible="securityVerification" :footer="null" :dialog-style="{ top: '190px' }"
    @cancel="securityCancel">
    <slide-verify ref="slideblock" @again="onAgain" @fulfilled="onFulfilled" @success="onSuccess" @fail="onFail"
      @refresh="onRefresh" slider-text="向右拖动滑块填充拼图" :accuracy="accuracy" :w="350" :h="175"
      :imgs="securityImgs"></slide-verify>
    <div>{{ msg }}</div>
  </a-modal>
</template>

<script>
export default {
  props: {
    securityVerification: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      msg: '',
      accuracy: 3,
      phoneNumber: '',
      securityImgs: [
        require('@/static/img/save1.jpg'),
        require('@/static/img/save2.jpg'),
        require('@/static/img/save3.jpg'),
        require('@/static/img/save4.jpg'),
        require('@/static/img/save5.jpg'),
      ]
    }
  },
  methods: {
    securityCancel() {
      this.$emit('update:securityVerification', false)
    },
    onSuccess(times) {
      this.$emit('update:securityVerification', false)
      this.$emit("SendMsg")
    },
    onFail() {
      this.msg = ''
    },
    onRefresh() {
      this.msg = ''
    },
    onFulfilled() {
    },
    onAgain() {
      console.log('检测到非人为操作的哦！');
      this.msg = 'try again';
      // 刷新
      this.$refs.slideblock.reset();
    },
  }
}
</script>
<style lang="less" scoped>
/deep/.ant-modal {
  .ant-modal-content {
    .ant-modal-body {
      padding: 24px;
    }
  }
}
</style>

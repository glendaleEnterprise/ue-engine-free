<template>
  <a-modal v-model="thisVisible" title="分享地址设置" :width="800" :Loading="Loading" @ok="handleOk" @cancel="handleCancel">
    <a-form ref="ruleForm" :model="form">
      <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="分享名称">
        <a-input v-model="form.shareTile" v-decorator="['shareTile', { rules: [{ required: true, message: '分享名称不能为空' }] }]" disabled />
      </a-form-item>
      <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="分享链接">
        <a-input v-model="form.shareLink" disabled />
      </a-form-item>
      <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="授权码">
        <a-input v-model="form.auth" />
      </a-form-item>

      <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="天数" readonly>
        <a-input-number v-model="form.effectiveDay" :min="0" :max="365" @change="onDayChange" readOnly />
      </a-form-item>

      <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="分享类型" readonly>
        <a-tree-select placeholder="分享类型" v-model="form.shareType" :tree-data="treesType" disabled />
      </a-form-item>

      <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="分享状态" readonly>
        <a-tree-select placeholder="分享状态" v-model="form.shareState" :disabled="IsStatuOpt" :tree-data="treesStatu">
        </a-tree-select>
      </a-form-item>

      <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="是否校验">
        <a-switch v-model="form.isVerify" v-decorator="[true, { valuePropName: 'checked', initialValue: true }]" checked-children="是" un-checked-children="否" />
      </a-form-item>
      <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="创建时间" readonly>
        {{ form.creationTime }}
      </a-form-item>
    </a-form>
  </a-modal>
</template>
<script>
import { saveShare } from '@/api/share'
export default {
  name: 'EditShare',
  methods: {},
  props: {
    editShare: { type: Object },
  },
  data () {
    return {
      IsStatuOpt: false,
      labelCol: {
        xs: { span: 24 },
        sm: { span: 5 },
      },
      form: this.editShare,
      treesStatu: [
        { title: '关闭', value: 0, key: 0 },
        { title: '分享', value: 1, key: 1 },
        { title: '永久分享', value: 200, key: 200 },
        { title: '超期', value: -1, key: -1, disabled: true },
      ],
      treesType: [
        { title: '视点', value: 1, key: 1 },
        { title: '漫游', value: 2, key: 2 },
        { title: '合模', value: 3, key: 3 },
        { title: '合模文件', value: 4, key: 4 },
        { title: '模型', value: 5, key: 5 },
      ],
      wrapperCol: {
        xs: { span: 24 },
        sm: { span: 16 },
      },
      thisVisible: true,
      Loading: false,
    }
  },
  filters: {
    //设备状态过滤方法
    filterVerify (val) {
      switch (val) {
        case true:
          return '不校验'
        case false:
          return '校验'
        default:
          return '未知状态'
      }
    },
  },
  mounted: function() {
    this.IsStateOpt()
  },
  methods: {
    IsStateOpt () {
      if (this.form.shareState == -1) {
        this.IsStatuOpt = true
      }
    },
    onDayChange () {

    },
    handleCancel () {
      this.$emit('update:visible', false)
    },
    handleOk () {
      const that = this
      that.Loading = true
      const formData = { ...that.form }
      saveShare(formData)
        .then(res => {
          that.$message.success('保存成功')
          //that.$emit('input', res.id)
          //that.$emit('nextStep')
          this.form.shareState = res.shareState
          this.$emit('update:visible', false)
        })
        .catch(err => {
          that.$message.error('保存失败')
        }
        )
        .finally(() => {
          that.Loading = false

        })
    },
    onChange () { },
    onCopy (e) {
      this.$message.success('内容已复制到剪切板！')
    },
    // 复制失败时的回调函数
    onError (e) {
      this.$message.error('抱歉，复制失败！')
    }
    // edit(data) {
    //   console.log(this.form)
    // },
  },
}
</script>

<style lang="less" scoped>
</style>

<template>
  <a-modal title="修改密码" :width="500" :visible="visible" :confirmLoading="confirmLoading" @ok="handleOk" @cancel="hide"
    :mask="passwordState == 0 ? true : false" :maskClosable="false">
    <a-form :form="form" v-if="passwordState == 0">
      <a-tabs size="small" v-model="tabKey">
        <a-tab-pane :key="1" :tab="'旧密码验证修改'">
          <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="当前密码">
            <a-input-password placeholder="当前密码"
              v-decorator="['currentPassword', { rules: [{ required: tabKey === 1, message: '当前密码不能为空' }, { min: 6, message: '密码至少为6位' }] }]" />
          </a-form-item>
          <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="新密码">
            <a-input-password placeholder="新密码"
              v-decorator="['newPassword', { rules: [{ required: tabKey === 1, message: '密码不能为空' }, { min: 6, message: '密码至少为6位' }, { validator: compareToRepPassword }] }]" />
          </a-form-item>
          <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="重复新密码">
            <a-input-password placeholder="重复新密码"
              v-decorator="['repPassword', { rules: [{ required: tabKey === 1, message: '密码不能为空' }, { min: 6, message: '密码至少为6位' }, { validator: compareToNewPassword }] }]" />
          </a-form-item>
        </a-tab-pane>
        <a-tab-pane :key="2" :tab="'手机验证修改'" v-if="$store.state.deploymentMethod != 'private'">
          <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="手机号">
            <a-space class="phone-show">
              <a-input v-model="currentUser.phoneNumber" :disabled="true" />
              <a-button type="link" @click="onSendSms" :disabled="smsButton" size="small">{{ smsText }}</a-button>
            </a-space>
          </a-form-item>
          <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="验证码">
            <a-input placeholder="验证码"
              v-decorator="['code', { rules: [{ required: tabKey === 2, message: '验证码不能为空' }] }]" />
          </a-form-item>
          <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="新密码">
            <a-input-password placeholder="新密码"
              v-decorator="['newPassword', { rules: [{ required: tabKey === 2, message: '密码不能为空' }, { min: 6, message: '密码至少为6位' }, { validator: compareToRepPassword }] }]" />
          </a-form-item>
          <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="重复新密码">
            <a-input-password placeholder="重复新密码"
              v-decorator="['repPassword', { rules: [{ required: tabKey === 2, message: '密码不能为空' }, { min: 6, message: '密码至少为6位' }, { validator: compareToNewPassword }] }]" />
          </a-form-item>
        </a-tab-pane>
      </a-tabs>
    </a-form>
    <a-form :form="form" v-else>
      <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="手机号">
        <a-space class="phone-show">
          <a-input v-model="currentUser.phoneNumber"/>
          <a-button type="link" @click="onSendSms" :disabled="smsButton" size="small">{{ smsText }}</a-button>
        </a-space>
      </a-form-item>
      <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="验证码">
        <a-input placeholder="验证码" v-decorator="['code', { rules: [{ required: tabKey === 2, message: '验证码不能为空' }] }]" />
      </a-form-item>
      <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="新密码">
        <a-input-password placeholder="新密码"
          v-decorator="['newPassword', { rules: [{ required: tabKey === 2, message: '密码不能为空' }, { min: 6, message: '密码至少为6位' }, { validator: compareToRepPassword }] }]" />
      </a-form-item>
      <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="重复新密码">
        <a-input-password placeholder="重复新密码"
          v-decorator="['repPassword', { rules: [{ required: tabKey === 2, message: '密码不能为空' }, { min: 6, message: '密码至少为6位' }, { validator: compareToNewPassword }] }]" />
      </a-form-item>
    </a-form>
  </a-modal>
</template>

<script>
import { changePasswordNew, sendVerifyCode } from '@/api/login'
import { mapGetters } from 'vuex'
export default {
  name: 'ChangePasswordModal',
  props: {
    passwordState: {
      type: Number,
      default: 0
    }
  },
  data() {
    return {
      smsText: '获取验证码',
      smsButton: false,
      timer: null, //定时器对象
      state: {
        time: 60,
        loginBtn: false,
        smsSendBtn: false,
      },
      tabKey: 1,
      labelCol: {
        xs: { span: 24 },
        sm: { span: 5 }
      },
      wrapperCol: {
        xs: { span: 24 },
        sm: { span: 16 }
      },
      visible: false,
      confirmLoading: false,
      form: this.$form.createForm(this),
    }
  },
  computed: {
    ...mapGetters({
      currentUser: 'userInfo',
    }),
  },
  methods: {
    show() {
      this.visible = true
    },
    hide() {
      this.visible = false
    },
    compareToRepPassword(rule, value, callback) {
      const form = this.form
      if (value && form.getFieldValue('repPassword')) {
        form.validateFields(['repPassword'], { force: true })
      }
      callback()
    },
    compareToNewPassword(rule, value, callback) {
      const form = this.form
      if (value.length >= 6 && value && value !== form.getFieldValue('newPassword')) {
        callback('两次输入密码不一致')
      } else {
        callback()
      }
    },
    //获取验证码
    onSendSms() {
      const that = this
      if (this.currentUser.phoneNumber) {
        that.smsButton = true
        sendVerifyCode(this.currentUser.phoneNumber, true).then((res) => {
          if (res.status) {
            that.$message.success('验证码已发送')
          } else {
            that.$message.error(res.message)
          }
        })
        that.timer = setInterval(() => {
          that.state.time--
          that.smsText = '获取验证码(' + that.state.time + '秒)'
          if (that.state.time <= 0) {
            that.smsButton = false
            that.smsText = '获取验证码'
            clearInterval(that.timer)
            that.state.time = 60
          }
        }, 1000)
      }
    },
    handleOk() {
      const that = this
      // 触发表单验证
      this.form.validateFields((err, values) => {
        // 验证表单没错误
        if (!err) {
          that.confirmLoading = true
          const { currentPassword, newPassword, code } = values
          changePasswordNew({ currentPassword, newPassword, code }).then(res => {
            that.$message.success('密码修改成功！')
            that.hide()
            that.$store.dispatch('Logout').then(() => {
              window.location.reload()
            }) 
          }).catch(() => {
          }).finally(() => {
            that.confirmLoading = false
          })
        }
      })
    },
  },
  destroyed() {
    clearInterval(this.timer)
  }
}
</script>

<style lang="less" scoped>
.phone-show {
  width: 100%;

  .ant-space-item:first-child {
    width: 69%;
  }
}
</style>
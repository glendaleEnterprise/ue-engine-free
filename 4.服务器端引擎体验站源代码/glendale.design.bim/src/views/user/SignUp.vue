<template>
  <div>
    <a-modal title="注册账号" :width="450" :visible="visible" :confirmLoading="confirmLoading" @ok="handleOk"
      @cancel="handleCancel">
      <a-form :form="form" class="form" :label-col="{ span: 6 }" :wrapper-col="{ span: 16 }">
        <a-form-item label="公司名称">
          <a-input placeholder="公司名称"
            v-decorator="['corporateName', { rules: [{ required: true, message: '公司名称不能为空' }] }]" />
        </a-form-item>
        <a-form-item label="手机号">
          <a-input placeholder="手机号"
            v-decorator="['phoneNumber', { rules: [{ required: true, message: '手机号码不能为空' }, { pattern: /^1[3456789]\d{9}$/, message: '输入手机号有误' },] }]" />
        </a-form-item>
        <a-form-item label="用户名">
          <a-input placeholder="用户名" v-decorator="['name', { rules: [{ required: true, message: '用户名不能为空' }] }]" />
        </a-form-item>
        <a-form-item label="登录密码">
          <a-input placeholder="登录密码" v-decorator="['password', { rules: [{ required: true, message: '登录密码不能为空' }] }]" />
        </a-form-item>
        <a-form-item label="验证码">
          <a-space>
            <a-input placeholder="验证码"
              v-decorator="['verifyCode', { rules: [{ required: true, message: '请输入验证码' }, { pattern: /^\d{6}$/, message: '输入验证码有误' }] }]">
            </a-input>
            <a-button @click="onSendSms" :disabled="smsButton" :style="{ fontSize: '12px' }">{{ smsText }}</a-button>
          </a-space>
        </a-form-item>
        <a-form-item v-bind="tailFormItemLayout">
          <a-checkbox
            v-decorator="['agreement', { rules: [{ required: true, message: '请先同意《使用协议》' }], valuePropName: 'checked' }]">
            我已阅读并同意
            <router-link :to="{ name: 'agreement' }" target="_blank">
              使用协议
            </router-link>
          </a-checkbox>
        </a-form-item>
      </a-form>
    </a-modal>
    <security-verification :securityVerification.sync="securityVerification" @SendMsg="SendMsg"
      v-if="securityVerification"></security-verification>
  </div>
</template>
<script>
import SecurityVerification from '@/components/SecurityVerification.vue'
import { sendVerifyCode, logon } from '@/api/login'
export default {
  name: 'SignUp',
  props: {
    signUpVisible: {
      type: Boolean,
      default: false
    }
  },
  components: {
    SecurityVerification
  },
  data() {
    return {
      visible: this.signUpVisible,
      confirmLoading: false,
      form: this.$form.createForm(this),
      time: 60,
      smsButton: false,
      smsText: '获取短信验证码',
      tailFormItemLayout: {
        wrapperCol: {
          xs: {
            span: 24,
            offset: 0,
          },
          sm: {
            span: 18,
            offset: 6,
          },
        },
      },
      securityVerification: false
    }
  },
  methods: {
    //获取验证码
    onSendSms() {
      const that = this
      const {
        form: { validateFields },
      } = this
      validateFields(['phoneNumber'], { force: true }, (err, values) => {
        if (!err) {
          that.securityVerification = true;
          that.phoneNumber = values.phoneNumber;
        }
      })
    },
    SendMsg() {
      const that = this;
      that.smsButton = true;
      sendVerifyCode(that.phoneNumber, false).then((res) => {
        if (res.status) {
          that.$message.success('验证码已发送')
        } else {
          that.$message.error(res.message)
        }
      })
      that.timer = setInterval(() => {
        that.time--
        that.smsText = '(' + that.time + '秒后获取)'
        if (that.time <= 0) {
          that.smsButton = false
          that.smsText = '获取短信验证码'
          clearInterval(that.timer)
          that.time = 60
        }
      }, 1000)
    },
    handleOk() {
      const {
        form: { validateFields },
        // LoginPhone,
      } = this
      const that = this;
      validateFields((err, values) => {
        if (!err) {
          const parame = {
            "userName": values.phoneNumber,
            "name": values.name,    //"【用户名】"
            "email": values.phoneNumber + '@123.com',    //"【默认用手机号就行】@123.com",
            "phoneNumber": values.phoneNumber,  //"【手机号】"
            "lockoutEnabled": true,
            "roleNames": ["manager"],
            "extraProperties": {
              "CorpName": values.corporateName,   // "【公司名】"
            },
            "password": values.password, //"【登录密码】"
            "Code": values.verifyCode
          }
          logon(parame)
            .then((res) => {
              that.$message.success('注册成功')
              that.$emit('PasswordLogin', { 'username': values.phoneNumber, 'password': values.password });
              that.$emit('update:signUpVisible', false)
            })
            .catch((err) => {
              that.$message.error('注册失败')
            })
            .finally(() => {
            })
        }
      })
    },
    handleCancel() {
      const that = this;
      that.$emit('update:signUpVisible', false)
    },
  },
}
</script>


<template>
  <a-row type="flex" justify="center">
    <a-col :span="12">
      <a-card :bordered="false">
        <div class="page-title">系统管理员信息</div>
        <a-form-model ref="ruleForm" :model="form" :rules="rules" :label-col="{ span: 7 }" :wrapper-col="{ span: 11 }">
          <a-form-model-item label="账号" v-show="false" >
            <span>{{ form.userName }}</span>
          </a-form-model-item>
          <a-form-model-item label="姓名" required prop="name">
            <a-input placeholder="请输入" v-model="form.name" />
          </a-form-model-item>
          <a-form-model-item label="绑定手机号" required prop="phoneNumber">
            <a-input-search placeholder="请输入手机号" v-model="form.phoneNumber" @search="onSend">
              <a-button slot="enterButton" style="padding: 5px; width: 120px" :disabled="smsButton">{{
                smsText
              }}</a-button>
            </a-input-search>
          </a-form-model-item>
          <a-form-model-item label="验证码" required prop="code">
            <a-input placeholder="请输入验证码" v-model="form.code" />
          </a-form-model-item>
          <a-form-model-item :wrapper-col="{ offset: 7 }">
            <a-button type="primary" @click="onSubmit"> {{$t('public.OK')}} </a-button>
            <!-- <a-button :style="{ marginLeft: '10px' }"> {{$t('list.cancel')}} </a-button> -->
          </a-form-model-item>
        </a-form-model>
      </a-card>
    </a-col>
  </a-row>
</template>
<script>
import { sendVerifyCode, changePhoneNumber } from '@/api/login'
 
export default {
  name: 'AdminSetting',
  data() {
    return {
      form:Object.assign({},this.$store.getters.userInfo) ,
      rules: {
        name: [
          { required: true, message: '请输入', trigger: 'blur' },
          { max: 10, message: '最长10个字符', trigger: 'blur' },
        ],
        phoneNumber: [{ required: true, message: '请输入', trigger: 'blur' }],
        code: [{ required: true, message: '请输入', trigger: 'blur' }],
      },
      state: {
        time: 60,
      },
      smsText: '获取验证码',
      smsButton: false,
      timer: null, //定时器对象
    }
  },
  methods: {
    verifyPhoneNumber(value) {
      if (value == '') {
        return true
      }
      var phonereg = /^1[3456789]\d{9}$/
      if (value.length == 11 && phonereg.test(value)) {
        return true
      }
      return false
    },
    onSend() {
      const that = this
      if (!that.verifyPhoneNumber(that.form.phoneNumber)) {
        that.$message.error('手机号输入有误!')
        return
      }
      sendVerifyCode(that.form.phoneNumber, false).then((res) => {
        if (res.status) {
          that.$message.success('验证码已发送')                  
        } else {
          that.$message.error(res.message)
        }
      })
      that.smsButton = true
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
    },
    onSubmit() {
      const that = this
      this.$refs.ruleForm.validate((valid) => {
        if (valid) {
          if (!that.verifyPhoneNumber(that.form.phoneNumber)) {
            that.$message.error('手机号输入有误!')
            return
          }
          const userData = {
            userName: that.form.userName,
            phoneNumber: that.form.phoneNumber,
            code: that.form.code,
            name: that.form.name,
          }
          changePhoneNumber(userData).then((res) => {
            if (res === 'ok') {
              that.$message.success('保存成功')
              that.form.code = ''
              this.$store.dispatch('GetInfo')
            } else {
              that.$message.error(res)
            }
          })
        } else {
          return false
        }
      })
    },
  },
}
</script>
<style scoped>
</style>
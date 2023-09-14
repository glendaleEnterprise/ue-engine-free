<template>
  <div>
    <a-modal title="" :visible="visible" :closable="false" :centered="true" :width="600">
      <template slot="footer">
        <a-button v-show="currentStep == 0" type="primary" :disabled="loginDisabled" @click="nextStep">下一步</a-button>
        <a-button v-show="currentStep == 1"
                  type="primary"
                  :disabled="loginDisabled"
                  @click="reLogin"
        >重新登录</a-button
        >
      </template>
      <div v-show="currentStep == 0">
        <div><big>许可协议</big></div>
        <a-spin tip="正在加载，请稍后..." :spinning="loading">
          <div class="box">
            <vue-scroll>
              <div>协议内容</div>
            </vue-scroll>
          </div>
        </a-spin>
        <div>
          <a-checkbox @change="onChange"> 我同意此协议 </a-checkbox>
        </div>
      </div>
      <div v-show="currentStep == 1">
        <div><big>修改密码</big></div>
        <div class="box" style="height: 490px; padding-top: 50px">
          <div style="color: red; padding-bottom: 30px; padding-left: 20px">
            注：为了账户的安全性，强烈建议立即修改初始密码，修改后重新登录
          </div>
          <a-form :form="form" :label-col="{ span: 6 }" :wrapper-col="{ span: 14 }">
            <a-form-item label="当前密码">
              <a-input-password
                placeholder="不能为空"
                v-decorator="[
                  'currentPassword',
                  {
                    rules: [
                      { required: true, message: '当前密码不能为空' },
                      { min: 6, message: '密码至少为6位' },
                    ],
                  },
                ]"
              />
            </a-form-item>
            <a-form-item label="新密码">
              <a-input-password
                placeholder="至少6位大小写字母、数字、特殊字符"
                v-decorator="[
                  'newPassword',
                  {
                    rules: [{ validator: compareToRepPassword }],
                  },
                ]"
              />
            </a-form-item>
            <a-form-item label="重复新密码">
              <a-input-password
                placeholder="不能为空"
                v-decorator="[
                  'repPassword',
                  {
                    rules: [{ required: true, message: '密码不能为空' }, { validator: compareToNewPassword }],
                  },
                ]"
              />
            </a-form-item>
            <a-form-item :wrapper-col="{ offset: 6 }">
              <a-button type="primary" @click="handleSubmit"> {{ $t('public.OK') }} </a-button>
            </a-form-item>
          </a-form>
        </div>
      </div>
    </a-modal>
  </div>
</template>
<script>
import { changePassword } from '@/api/login'
export default {
  name: 'Agreement',
  props: {
    isInit: {
      type: Boolean,
      defaultValue: false,
    },
  },
  data() {
    return {      
      visible: this.isInit,
      loginDisabled: true,      
      loading: true,
      currentStep: 0, //当前步骤
      form: this.$form.createForm(this, { name: 'changePassword' }),
    }
  },
  created() {
    this.getData()
  },
  mounted() {},
  methods: {
    async getData() {
      this.loading = true      
    },    
    onChange(e) {
      this.loginDisabled = !e.target.checked
    },
    //下一步
    nextStep() {
      this.currentStep = 1
    },
    compareToRepPassword(rule, value, callback) {
      const form = this.form
      //var reg = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$/
      var reg = /^(?![A-z0-9]+$)(?=.[^%&',;=?$\x22])(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).{6,}$/
      if (!reg.test(value)) {
        callback('至少6位大小写字母、数字、特殊字符')
      }
      callback()
    },
    compareToNewPassword(rule, value, callback) {
      const form = this.form
      if (value && value !== form.getFieldValue('newPassword')) {
        callback('两次输入密码不一致')
      }
      callback()
    },
    handleSubmit(e) {
      const that = this
      e.preventDefault()
      // 触发表单验证
      this.form.validateFields((err, values) => {
        // 验证表单没错误
        if (!err) {
          const { currentPassword, newPassword } = values
          changePassword({ currentPassword, newPassword })
            .then((res) => {               
              that.$message.success('密码修改成功！')               
            })
            .catch(() => {})
            .finally(() => {})
        }
      })
    },
    reLogin() {
      this.visible = false
      this.$emit('change')
      this.$store.dispatch('Logout')
    },
  },
}
</script>
<style scoped lang="less">
.box {
  margin: 10px 0;
  height: 50vh;
  border: 1px solid #dfdfdf;
  background-color: #fff;
  padding: 5px;
}

/deep/.ant-modal-content {
  background: #e5e5e5;
  // padding: 0 20px;
}
</style>

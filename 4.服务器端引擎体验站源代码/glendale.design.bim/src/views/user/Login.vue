<template>
  <div class="login-box" ref="wrap">
    <a-modal title="登录" :visible="loginVisible" :confirm-loading="confirmLoading" @cancel="handleCancel" :width="380"
      :footer="null" :getContainer="() => this.$refs.wrap" :maskClosable="false">
      <a-form id="formLogin" v-if="loginType == 0" class="user-layout-login" ref="formLogin" :form="form"
        @submit="handleSubmit">
        <a-form-item>
          <a-input size="large" type="text" placeholder="手机号" v-decorator="[
            'username',
            { rules: [{ required: true, message: '请输入用户名' }], validateTrigger: 'change' },
          ]">
            <a-icon slot="prefix" type="user" :style="{ color: '#05a081' }" />
          </a-input>
        </a-form-item>
        <a-form-item>
          <a-input-password size="large" placeholder="请输入用户密码" v-decorator="[
            'password',
            { rules: [{ required: true, message: '请输入用户密码' }], validateTrigger: 'blur' },
          ]">
            <a-icon slot="prefix" type="lock" :style="{ color: '#05a081' }" />
          </a-input-password>
        </a-form-item>
        <a-form-item>
          <a-button size="large" type="primary" htmlType="submit" class="login-button" :disabled="state.loginBtn"
            :loading="state.loginBtn">登录</a-button>
        </a-form-item>
        <a-alert v-if="isLoginError" type="error" showIcon style="margin-bottom: 12px" :message="loginError" />
      </a-form>
      <a-form id="formPhone" v-else class="user-layout-login" ref="formPhone" :form="formPhone">
        <a-form-item>
          <a-input size="large" type="text" placeholder="手机号" v-decorator="[
            'phoneNumber',
            {
              rules: [
                { required: true, message: '请输入手机号' },
                {
                  validator: validatePhone,
                },
              ],
              validateTrigger: 'change',
            },
          ]">
            <a-icon slot="prefix" type="phone" :style="{ color: '#05a081' }" />
          </a-input>
        </a-form-item>
        <a-form-item>
          <div style="display: flex">
            <div style="width: 236px">
              <a-input placeholder="验证码" size="large" v-decorator="[
                'verifyCode',
                {
                  rules: [
                    { required: true, message: '请输入验证码' },
                    {
                      validator: validateCode,
                    },
                  ],
                },
              ]">
                <a-icon slot="prefix" type="code" :style="{ color: '#05a081' }" />
              </a-input>
            </div>
            <div>
              <a-button type="link" @click="onSendSms" :disabled="smsButton" size="small"
                :style="{ fontSize: '12px', color: smsButton ? '#c4c7c8' : '' }">{{ smsText }}
              </a-button>
            </div>
          </div>
        </a-form-item>
        <a-form-item>
          <a-button size="large" type="primary" class="login-button" :disabled="state.loginBtn" :loading="state.loginBtn"
            @click="phoneSubmit">登录</a-button>
        </a-form-item>
      </a-form>
      <a-space class="login-tips" v-if="$store.state.deploymentMethod != 'private'">
        <a-button type="link" @click="loginMethodChange">
          {{ loginType == 0 ? '短信登录' : '密码登录' }}
        </a-button>
        <a-space class="tips-right">
          <!-- <a-button type="link" @click="forgotPassword">
            忘记密码
          </a-button> -->
          <a-button type="link" @click="signUp">
            注册账号
          </a-button>
        </a-space>
      </a-space>
    </a-modal>
    <change-password ref="changePassword" :passwordState="1" />
    <sign-up :signUpVisible.sync="signUpVisible" v-if="signUpVisible" @PasswordLogin="PasswordLogin"></sign-up>
    <security-verification :securityVerification.sync="securityVerification" @SendMsg="SendMsg"
      v-if="securityVerification"></security-verification>
  </div>
</template>
<script>
import SecurityVerification from '@/components/SecurityVerification.vue'
import { mapActions, mapState, mapGetters } from 'vuex'
import { getFileByBlobName } from '@/api/file'
import { sendVerifyCode } from '@/api/login'
import ChangePassword from '../../components/GlobalHeader/modules/ChangePasswordModal'
import store from '@/store'
import SignUp from './SignUp'
export default {
  props: {
    loginVisible: {
      type: Boolean,
    }
  },
  components: {
    ChangePassword,
    SignUp,
    SecurityVerification
  },
  data() {
    return {
      loginBtn: false,
      weChatQr: '',
      isLoginError: false,
      loginError: undefined,
      requiredTwoStepCaptcha: false,
      stepCaptchaVisible: false,
      form: this.$form.createForm(this),
      formPhone: this.$form.createForm(this),
      state: {
        time: 60,
        loginBtn: false,
        smsSendBtn: false,
      },
      smsText: '获取验证码',
      smsButton: false,
      timer: null, //定时器对象
      loginType: 0, //密码登录
      loginStatus: true,
      visible: this.loginVisible,
      confirmLoading: false,
      signUpVisible: false,
      phoneNumber: undefined,
      securityVerification: false
    }
  },
  created() {
    const that = this;
    if (that.$router.currentRoute.path == '/login') {
      window.addEventListener('keydown', this.keyDown)
      this.loginStatus = this.first;
      this.loginStatus ? this.demoLogin() : null
    }
  },
  computed: {
    ...mapState({
      // 动态主路由
      mainMenu: (state) => state.permission.addRouters,
    }),
    ...mapGetters(['roles', 'pw_status', 'first']),
  },
  methods: {
    getFileByBlobName,
    ...mapActions(['Login', 'LoginPhone', 'Logout']),
    keyDown(e) {
      var that = this
      if (e.keyCode == 13 && that.loginType == 0) {
        that.handleSubmit()
      }
      if (e.keyCode == 13 && that.loginType == 1) {
        that.phoneSubmit()
      }
    },
    //密码登录
    handleSubmit(e) {
      e.preventDefault()
      const {
        form: { validateFields },
        state,
        PasswordLogin
      } = this
      state.loginBtn = true
      validateFields(['username', 'password'], (err, values) => {
        if (!err) {
          const loginParams = { ...values }
          PasswordLogin(loginParams)
        } else {
          setTimeout(() => {
            state.loginBtn = false
          }, 600)
        }
      })
    },
    PasswordLogin(loginParams) {
      const that = this;
      that.Login(loginParams)
        .then((res) => that.loginSuccess(loginParams, 1))
        .catch((err) => that.requestFailed(err))
        .finally(() => {
          that.state.loginBtn = false
        })
    },
    //获取验证码
    onSendSms() {
      const that = this
      const {
        formPhone: { validateFields },
        Login,
      } = this
      validateFields(['phoneNumber'], { force: true }, (err, values) => {
        if (!err) {
          that.phoneNumber = values.phoneNumber
          that.securityVerification = true;
        }
      })
    },
    SendMsg() {
      const that = this;
      that.smsButton = true
      sendVerifyCode(that.phoneNumber, true).then((res) => {
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
    },
    //验证码登录
    phoneSubmit() {
      const {
        formPhone: { validateFields },
        state,
        LoginPhone,
      } = this
      state.loginBtn = true
      validateFields(['phoneNumber', 'verifyCode'], { force: true }, (err, values) => {
        if (!err) {
          const loginParams = {
            phone: values.phoneNumber,
            code: values.verifyCode,
          }
          LoginPhone(loginParams)
            .then((res) => {
              this.loginSuccess(loginParams, 2)
            })
            .catch((err) => this.requestFailed(err))
            .finally(() => {
              state.loginBtn = false
            })
        } else {
          setTimeout(() => {
            state.loginBtn = false
          }, 600)
        }
      })
    },
    loginSuccess(res, ty) {
      const that = this
      this.isLoginError = false
      that.$store.dispatch('SetProject', '')
      var val = res.phone || res.username
      store
        .dispatch('GetInfo')
        .then(res => {
          const project = store.getters.currProject
          const token = store.getters.token
          // 根据用户权限信息生成可访问的路由表
          store.dispatch('GenerateRoutes', { token, ...res, project }).then(() => {
            setTimeout(() => {
              if (!that.mainMenu[0]) {
                that.$router.replace({ path: '/404' }, () => {
                  if (that.loginStatus) location.reload()
                })
              } else {
                if (that.mainMenu[0].children.find(e => e.path == '/project')) {
                  if (that.$router.currentRoute.path == '/project') {
                    that.$router.go(0)
                  } else {
                    // store.commit('SET_ROLES', [])
                    store.getters.addRouters.forEach(r => {
                      that.$router.addRoute(r)
                    })
                    that.$router.push('/project')
                    // this.$store.dispatch('SetProject', '3a0a7124-b846-f2cd-6a95-ec55d8942b15')
                    // this.$router.replace({ path: '/business/model' })
                    // that.$router.replace({ path: '/business/model' }, () => {
                    //   location.reload()
                    // })
                  }
                } else {
                  that.$router.replace({ path: that.mainMenu[0].children[0].children ? that.mainMenu[0].children[0].children[0].path : that.mainMenu[0].children[0].path }, () => {
                    location.reload()
                  })
                }
              }
            }, 1000)
          })
        })
      //正常登录
      if (val != 'guest' && val != 'user') {
        that.visible = false;
        that.$emit('update:loginVisible', that.visible)
      }
    },
    requestFailed(err) {
      const that = this
      this.isLoginError = true
      if (typeof err.response == 'undefined') {
        this.loginError = '无效的请求'
      } else {
        this.loginError = err.response.data.error_description
      }

      setTimeout(() => {
        that.isLoginError = false
      }, 5000)

      // this.$notification['error']({
      //   message: '错误',
      //   description: ((err.response || {}).data || {}).message || '请求出现错误，请稍后再试',
      //   duration: 4
      // })
    },
    //公开项目
    demoLogin() {
      const that = this
      that.state.loginBtn = true
      this.Login(that.$store.state.defaultUser)
        .then((res) => this.loginSuccess(that.$store.state.defaultUser, 0))
        .catch((err) => this.requestFailed(err))
        .finally(() => {
          that.state.loginBtn = false
        })
    },
    validatePhone(rule, value, callback) {
      const reg = /^[1][3,4,5,6,7,8,9][0-9]{9}$/
      if (value && !reg.test(value)) {
        callback('请输入有效手机号')
      }
      callback()
    },
    validateCode(rule, value, callback) {
      const reg = /^\d{6}$/
      if (value && !reg.test(value)) {
        callback('请输入合法验证码')
      }
      callback()
    },
    loginMethodChange() {
      this.loginType == 0 ? this.loginType = 1 : this.loginType = 0;
    },
    handleCancel() {
      this.visible = false;
      this.$emit('update:loginVisible', this.visible)
    },
    forgotPassword() {
      this.$refs.changePassword.show()
    },
    signUp() {   //注册账号
      this.signUpVisible = true;
    }
  },
  destroyed() {
    window.removeEventListener('keydown', this.keyDown)
  },
}
</script>

<style lang="less" scoped>
.login-box {
  .login-tips {
    width: 100%;
    justify-content: space-between;

    .tips-right {
      /deep/.ant-btn {
        padding: 0 5px;
      }
    }
  }
}

.tab-box {
  height: 40px;
  border-bottom: 1px solid #dfdfdf;
  display: flex;
  margin-bottom: 28px;

  .title {
    width: 90px;
    text-align: center;
    height: 40px;
    line-height: 40px;
    margin: 0px 10px;
    cursor: pointer;
  }

  .active {
    border-bottom: 2px solid #05a081;
    color: #05a081;
  }
}

.user-layout-login {
  margin-top: 10px;
  padding: 0 10px;

  label {
    font-size: 14px;
  }

  .forge-password {
    font-size: 14px;
  }

  button.login-button {
    padding: 0 15px;
    font-size: 16px;
    height: 40px;
    width: 100%;
  }

  .content {
    margin-top: 5px;
    display: flex;
    justify-content: space-between;
    font-size: 8px;
    color: #ddd;

    /deep/.ant-checkbox+span {
      font-size: 8px;
    }
  }
}

.user-layout-footer {
  position: absolute;
  right: 0;
  bottom: 0;

  img {
    height: 40px;
  }
}
</style>

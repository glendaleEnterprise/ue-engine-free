import qs from 'qs'
import request from '@/utils/request'
const userApi = {
  Login: `/connect/token`,
  Logout: `/api/account/logout`,
  UserInfo: `/api/identity/my-profile`,
  SignUp: `/api/app/user-register/register`
}

/**
 * 注册账号
 */
export function logon(parameter) {
  return request({
    url: userApi.SignUp,
    method: 'post',
    data: parameter
  })
}

/**
 * 密码登录
 * parameter: {
 *     username: '',
 *     password: '',
 *     remember_me: true,
 *     captcha: '12345'
 * }
 * @param parameter
 * @returns {*}
 */
export function login(parameter) {
  const formData = qs.stringify(Object.assign({
    grant_type: process.env.VUE_APP_GRANT_TYPE,
    scope: process.env.VUE_APP_SCOPE,
    client_id: process.env.VUE_APP_CLIENT_ID,
    client_secret: process.env.VUE_APP_CLIENT_SECRET
  }, parameter))
  return request({
    url: userApi.Login,
    method: 'post',
    data: formData
  })
}

/**
 * 手机验证码登录
*/
export function loginPhone(parameter) {
  const formData = qs.stringify(Object.assign({
    grant_type: process.env.VUE_APP_GRANT_TYPE_Phone,
    scope: process.env.VUE_APP_SCOPE_Phone,
    client_id: process.env.VUE_APP_CLIENT_ID_Phone,
    client_secret: process.env.VUE_APP_CLIENT_SECRET_Phone
  }, parameter))
  return request({
    url: userApi.Login,
    method: 'post',
    data: formData
  })
}


export function getInfo() {
  return request({
    url: userApi.UserInfo,
    method: 'get',
    headers: {
      'Content-Type': 'application/json;charset=UTF-8'
    }
  })
}
export function changePassword(formData) {
  return request({
    url: `/api/identity/my-profile/change-password`,
    // url: `/api/app/user-profile/change-password`,
    method: 'post',
    data: formData
  })
}

export function changePasswordNew(formData) {
  return request({
    url: `/api/app/user-profile/change-password`,
    // url: `/api/app/user-profile/change-password`,
    method: 'post',
    data: formData
  })
}
export function logout() {
  return request({
    url: userApi.Logout,
    method: 'get',
    headers: {
      'Content-Type': 'application/json;charset=UTF-8'
    }
  })
}

export function upInfo(formData) {
  return request({
    url: userApi.UserInfo,
    method: 'put',
    data: formData
  })
}

//获取验证码
export function sendVerifyCode(phone, isLogin) {
  return request({
    url: `/api/app/sms/send-verify-code?phoneNumber=${phone}&isLogin=${isLogin}`,
    method: 'post',
  })
}

//修改手机号
export function changePhoneNumber(params) {
  return request({
    url: `/api/app/user-profile/phone-number`,
    method: 'put',
    params
  })
}

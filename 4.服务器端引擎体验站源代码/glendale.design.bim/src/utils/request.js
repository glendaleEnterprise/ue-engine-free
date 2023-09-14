import axios from 'axios'
import store from '@/store'
import storage from 'store'
import notification from 'ant-design-vue/es/notification'
import { VueAxios } from './axios'
import { ACCESS_TOKEN } from '@/store/mutation-types'

// 创建 axios 实例
const request = axios.create({
  // API 请求的默认前缀
  baseURL: '',
  timeout: 10 * 60 * 1000, // 请求超时时间 
  withCredentials: false
})

// 异常拦截处理器
const errorHandler = (error) => {

  if (error.response) {
    if ([500, 400, 403].some(x => x === error.response.status)) {
      const data = error.response.data       
      notification.error({
        message: '错误',
        description: data.message || (data.error.validationErrors && data.error.validationErrors[0].message) || data.error.message || data.error_description
      })
    }
    if (error.response.status === 401 && !(data.result && data.result.isLogin)) {
      notification.error({
        message: '登录验证',
        description: '登录超时，请重新登录'
      })
      // 从 localstorage 获取 token
      const token = storage.get(ACCESS_TOKEN)
      if (token) {
        store.dispatch('Logout').then(() => {
          setTimeout(() => {
            window.location.reload()
          }, 1500)
        })
      }
    }
  }
  return Promise.reject(error)
}

// request interceptor
request.interceptors.request.use(config => {  
  config.baseURL =store.state.baseUrl
  const token = storage.get(ACCESS_TOKEN)
  // 如果 token 存在
  // 让每个请求携带自定义 token 请根据实际情况自行修改
  if (token) {
    config.headers['Authorization'] = `Bearer ${token}`
  }
  return config
}, errorHandler)

// response interceptor
request.interceptors.response.use((response) => {
  return response.data
}, errorHandler)

const installer = {
  vm: {},
  install (Vue) {
    Vue.use(VueAxios, request)
  }
}

export default request

export {
  installer as VueAxios,
  request as axios
}
